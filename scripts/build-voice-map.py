"""Rebuild voice metadata from hash-identified local Steam PAKs (no audio output).

Python 3.11+, stdlib only. --ffmpeg optionally decodes every selected WAV via stdin.
The generated JSON contains paths, lengths and hashes, never recordings or XML.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import struct
import subprocess
import xml.etree.ElementTree as ET

DATA_HASH = '6926c286b8e44dba9244723fbcd153a3ee8fc28633e3c22cc49c27d206c96d50'
LANGUAGES = {
    'spanish': '22fcebb4b0846420bac911812bd66f42c43c54fc6f5575a4319c6aaf993c76e2',
    'italian': 'a5f11c3013ec5beef25ae22768eeb3ce96145f95bfd1906bb26f280ea5c971de',
    'english': 'a2b6ce5281a84824a25f42aed592974785b538830e6d38ce0e3bcca8d0bd17f3',
}
PREFIX = 'CURRENTLANG\\VOICES\\'


def digest(path):
    with path.open('rb') as f:
        return hashlib.file_digest(f, 'sha256').hexdigest()


def index(path):
    result = {}
    with path.open('rb') as f:
        header = f.read(40)
        if not header.startswith(b'HMMSYS PackFile\n\x1a') or len(header) != 40:
            raise ValueError('Invalid PAK header')
        count = struct.unpack_from('<I', header, 32)[0]
        if not 0 < count <= 100000:
            raise ValueError('Invalid entry count')
        previous = ''
        for _ in range(count):
            length, prefix = struct.unpack('<BB', f.read(2))
            if prefix > min(length, len(previous)):
                raise ValueError('Invalid name prefix')
            name = (previous[:prefix] + f.read(length-prefix).decode('latin1')).upper()
            offset, size = struct.unpack('<II', f.read(8))
            if name in result or offset + size > path.stat().st_size:
                raise ValueError('Invalid entry bounds')
            result[name] = offset, size
            previous = name
        if min(o for o, _ in result.values()) < f.tell():
            raise ValueError('Overlapping index')
    return result


def read(stream, entry):
    offset, size = entry
    stream.seek(offset)
    data = stream.read(size)
    if len(data) != size:
        raise ValueError('Truncated entry')
    return data


def build(root, ffmpeg=None):
    data = root / 'Packs/data.pak'
    if digest(data) != DATA_HASH:
        raise ValueError('Unknown data.pak')
    definitions = []
    with data.open('rb') as f:
        for name, entry in sorted(index(data).items()):
            if name.startswith('DATA\\SOUND ENTITIES\\VOICE') and name.endswith('.XML'):
                xml = ET.fromstring(read(f, entry))
                refs = [(s.get('file', '').upper().replace('/', '\\'), s.get('frequency'))
                        for s in xml.iter('sound')]
                refs = [(p, freq) for p, freq in refs if p.startswith(PREFIX)]
                definitions.append((xml, refs))
    assert sum(len(refs) for _, refs in definitions) == 393
    output = {'format': 1, 'data_pak_sha256': DATA_HASH, 'languages': []}
    for language, pak_hash in LANGUAGES.items():
        pak = root / 'local' / (language + '.pak')
        if digest(pak) != pak_hash:
            raise ValueError('Unknown ' + language + '.pak')
        entries = index(pak)
        pairs = {}
        for xml, refs in definitions:
            missing = []
            for dest, freq in refs:
                if dest in entries:
                    continue
                bare = dest[len(PREFIX):]
                if language == 'english' and bare in entries:
                    pairs[dest] = bare
                else:
                    missing.append((dest, freq))
            if not missing:
                continue
            # Reordering is permitted only inside one equal-weight UnitOrder list.
            if (xml.get('type') != 'UnitOrder' or len(xml.findall('files')) != 1
                    or len(list(xml.iter('sound'))) != len(missing)
                    or len({freq for _, freq in missing}) != 1):
                raise ValueError('Ambiguous sound list')
            groups = {p[len(PREFIX):].split('\\')[0] for p, _ in missing}
            if len(groups) != 1:
                raise ValueError('Mixed groups')
            group = groups.pop()
            prefix = ('' if language == 'english' else PREFIX) + group + '\\'
            # English CHERO also has 6..13.wav. These are not substitutes for 1..5.
            candidates = sorted(p for p in entries if p.startswith(prefix)
                                and p.endswith('.WAV')
                                and not re.fullmatch(r'[0-9]+\.WAV', p[len(prefix):]))
            if len(candidates) != len(missing):
                raise ValueError('Not a one-to-one phrase set: ' + group)
            if any(not re.fullmatch(r'[1-9][0-9]*\.WAV', p.rsplit('\\', 1)[1]) for p, _ in missing):
                raise ValueError('Expected numeric requests')
            for (dest, _), source in zip(sorted(missing, key=lambda r: int(r[0].rsplit('\\', 1)[1][:-4])), candidates):
                pairs[dest] = source
        expected = 393 if language == 'english' else 188
        if len(pairs) != expected:
            raise ValueError('Unexpected coverage')
        rows = []
        with pak.open('rb') as f:
            for dest, source in sorted(pairs.items()):
                payload = read(f, entries[source])
                if payload[:4] != b'RIFF' or payload[8:12] != b'WAVE' or struct.unpack_from('<I', payload, 4)[0]+8 != len(payload):
                    raise ValueError('Invalid WAV: ' + source)
                if ffmpeg:
                    decoded = subprocess.run([ffmpeg, '-nostdin', '-hide_banner', '-loglevel', 'error', '-xerror',
                                              '-i', 'pipe:0', '-map', '0:a:0', '-f', 's16le', 'pipe:1'],
                                             input=payload, capture_output=True, check=True)
                    if not decoded.stdout:
                        raise ValueError('Empty audio: ' + source)
                rows.append({'destination': dest.replace('\\', '/'), 'source': source,
                             'size': len(payload), 'sha256': hashlib.sha256(payload).hexdigest()})
        if digest(pak) != pak_hash:
            raise ValueError('PAK changed during generation')
        output['languages'].append({'language': language, 'pak_sha256': pak_hash, 'files': rows})
    if digest(data) != DATA_HASH:
        raise ValueError('data.pak changed')
    return output


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--game-root', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--ffmpeg')
    args = parser.parse_args()
    if args.output.exists():
        parser.error('Output must not exist; preserve previous evidence')
    result = build(args.game_root, args.ffmpeg)
    with args.output.open('x', encoding='utf-8', newline='\n') as f:
        f.write(json.dumps(result, ensure_ascii=True, indent=2) + '\n')
    print(json.dumps({'sha256': digest(args.output), 'decoded': bool(args.ffmpeg),
                      'counts': {x['language']: len(x['files']) for x in result['languages']}}))

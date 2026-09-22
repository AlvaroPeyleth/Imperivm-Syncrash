# Roadmap toward Syncrash

Syncrash may eventually provide one installation and diagnostic experience while keeping AntiCrasher and Syncro as separately identifiable components. Combining them depends on evidence that each works and that they can coexist without creating different simulation states between players.

## 1. Reliable observation

Repair the local game observer. A session is covered only when it identifies the correct process, confirms that exception capture is attached and records valid samples. The observer must distinguish a normal exit, a crash and a desync, and report when capture failed.

## 2. Validate AntiCrasher v2

Audit the second reconstructed crash and the three guarded call sites, including the x86 calling contract, list removal, object lifetime and later state. Keep the current V2 build unchanged as a reference. A separate diagnostic build may count rejected references, but a rejection alone is not proof that a crash was prevented.

## 3. Establish a causal Syncro candidate

Identify the script and bytecode the game actually runs. Review the complete meaning and flow of the suspected uninitialized boolean. Test whether changing its previous byte changes a decision, a command and the resulting game state under otherwise equal conditions. Verify the corresponding resource in Steam vanilla independently of any Community Mod resource.

## 4. Test coexistence

Compare identified executable and resource variants on both players' computers. Test AntiCrasher with equal resources on both sides, then test a validated Syncro candidate installed identically on both sides. Determine whether an AntiCrasher rejection can leave the two simulations with different states. Preserve paired logs and the earliest observable divergence.

## 5. Prepare a reversible distribution

Only after those checks, build a package that verifies exact supported hashes, backs up originals, installs selected components and restores each one independently. Publish a compatibility matrix and clear limits. A shared installer does not imply that both fixes must be merged into one binary.

Router port setup, NAT traversal and relay networking are separate future work. They do not establish simulation synchronization.

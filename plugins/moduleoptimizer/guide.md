# ModuleOptimizer

Finds the best module combination for the targets you set — and can apply it for you,
through the game's own equip flow.

![Module optimizer](https://cdn.revette.io/plugins/moduleoptimizer/media/module-optimizer.png)

## How to use

1. Install the plugin and launch the game **Modded**.
2. Open the optimizer from the Stellar overlay and set your **targets** — the attributes you
   want and any minimums (e.g. *Life Wave ≥ 20*).
3. Run the optimizer: it enumerates and scores module combinations from your inventory
   (5 module slots as of game patch 3.7) and shows the best candidates.
4. Review the suggested setup and **apply** it. You approve the plan — the plugin equips
   through the game's own equip flow, never bypassing its checks.

## Tips

- Apply is verified: after the plan runs, the equipped set is re-checked and any equip the
  server ignored is re-issued once; persistent failures name the stuck slots.
- Attribute minimums are floor-aware — a scarce attribute won't be crowded out of the pool by
  an abundant co-target.

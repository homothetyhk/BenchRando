# Bench Rando

This is a randomizer connection mod for randomizing benches. BenchRando settings can be toggled from the Connections menu.

## Randomized Items
The *Randomized Items* setting determines whether benches are randomized among the other items/locations.
- None: benches are not randomized as items/locations
- Warp Unlocks: benches are randomized as items/locations. The "item" unlocks its corresponding bench as a warp point. The "location" is obtained by resting at the corresponding bench. Resting will not unlock the bench to warp.
- Rest And Warp Unlocks: benches are randomized as items/locations. The "item" unlocks the corresponding bench as a warp point, and unlocks resting at the bench. The "location" is obtained by inspecting the bench before it is unlocked, or by resting at it after it is unlocked. **Resting at the bench is not possible before it is unlocked in this mode.**

The *Black Egg Temple* and *Godhome Roof* benches are not randomized under any settings.

Randomized benches will appear **purple** in the Benchwarp menu. This indicates that they cannot be unlocked by the normal method of resting. You can see the name of the bench or the name of its scene by using Benchwarp's setting toggle. 

With this setting active, Salubra's Blessing will be given at the start of the randomizer, to ensure the player has soul access after warping to a new bench. This can be logically required in some circumstances.

## Randomize Bench Spots

This setting randomizes the spots which have benches, replacing base game benches and adding new benches. BenchRando currently adds **102** new bench spots to the base **49** bench spots, for a total of **151** possible spots. The randomization guarantees that each of the Benchwarp area categories has at least **1** bench, and no more than **6** benches. It also guarantees that the total number of benches is between **45** and **55**.

The randomization is uniform in area: each area is equally likely to be chosen to receive a bench, and then the bench is chosen uniformly from the list of benches for said area.

The *Black Egg Temple* and *Godhome Roof* benches are not randomized under any settings.

## New Bench Style

This setting determines the style of benches created by **Randomize Bench Spots**. This affects the near and far appearance of the bench, and other factors such as its height off the ground.
- Default: benches have preselected styles, chosen on the basis of aesthetics.
- Random Coupled: each new bench receives a random style. The near and far styles will be the same.
- Random Decoupled: each new bench receives a random near style, and a random far style.

## Bench Group

This affects the bench group for purposes of split group rando. Please see the split group settings documentation for Randomizer 4 for more information.
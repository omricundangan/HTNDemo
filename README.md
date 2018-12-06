# HTNDemo
Basic implementation of a Hierarchical Task Network (HTN) in the form of a Unity minigame.

## Gameplay
The player plays against an AI-controlled Agent and controls the Blue circle, collecting the rotating items in the alcoves on the edge of the screen, while avoiding the patrolling enemy AI. The goal is to collect the most items while avoiding getting caught. Both players possess a Teleport Trap holding 2 uses, which can be used by pressing the 'Space' key. Doing so will teleport the closest enemy/agent

## Implementation
The opposing Agent is controlled by a basic Hierarchical Task Network. Implementation was based off of [this section](http://www.gameaipro.com/GameAIPro/GameAIPro_Chapter12_Exploring_HTN_Planners_through_Example.pdf) in Game AI Pro. The implemented pseudocode described in the paper can be found in the [EnemyAgentPlanner class](https://github.com/dlrht/HTNDemo/blob/master/Assets/Scripts/EnemyAgentPlanner.cs) in the MakePlan() method.

The Enemy Agent AI has a HTN Domain as follows:

![HTN Domain](https://i.imgur.com/uFAtVfi.png)

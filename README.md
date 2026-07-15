# Quival
![Screenshot](/ReadmeImages/screenshot.png)
A digital TCG where nobody goes first!

## How To Download
Your best bet is always going to be to head over to [the releases page](https://github.com/TL1227/Quival/releases) page and just download the latest version.

It is currently **windows only** as the client is built using WPF. (Though this is going to change as the project develops)

You could also build it from source using visual studio. Just open up the visual studio solution file and do all the usual visual studio things.

## How To Run
To get a game going you go into the Server directory and run QuivalServer.exe
Once you've done this open up MachineName.txt and add the machine name you can see on the Quival server's console.
Then go to the Client directory and run Quival.exe once for each player.
There is now a main menu when you start up.
To get into a match just click Random Match and it will either find a match or create a new one.

# Game Rules

_Note: Quival is pretty early in development and many things are likely to change!_

Each player starts the game with 7 cards in hand and 20 life points.

The game is split up into _Turns_ and _Rounds_.

At the start of each turn each player draws a card and their resources are reset (whatever it ends up being)
Players will then play 5 rounds. On a round, each player can submit one action.
An action could be summoning a card, moving a creature to the _Block Zone_, _Marking_ a creature, or attacking.

Once both players have submitted their actions both cards are reavealed and played at the same time.

Once the action's effects have been resolved, we move on to the next round.

After round 5 is played, the round counter is reset and we move on to the second turn.

## Round Effects
Some cards will have special abilities that change depending on the round they're played.
For example _"Deal 2 damage to any target. If this card is played on round 4, deal 4 damage instead."_

## Combat Actions
During each round you can have a summoned creature perform one of the following combat actions.
They cannot perform another action until the next Turn.

### Attack
When you attack with a creature, you target the opponent's life directly unless there is a creature in the opponent's Block Zone.
If a creature is in the Block Zone then both creatures attack each other. 

### Move to Block Zone
One way to block is you move a creature to the block zone. This takes up one round.
While in the block zone the creature will remain there until they are killed, the turn is over, or another creature moves into the blockzone pushing them out.

### About creature Health
Unlike some other games, health is persistent. _(NOTE: This might just persist during a single turn, I haven't decided yet)_
So for example if your 4/4 creature attacks and the opponent has a 2/2 in the block zone.
You will kill their creature and have a 4/2 creature.

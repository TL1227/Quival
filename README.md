# Quival
A digital TCG where nobody goes first!

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

### Marking
Another way to block is to have your creature "Mark" an opponent's creature. 
What this means is that if the marked creature is to attack your creature will block it. 
You can only mark one creature at a time and it lasts until the Turn is over.

### About creature Health
Unlike some other games, health is persistent. _(NOTE: This might just persist during a single turn, I haven't decided yet)_
So for example if your 4/4 creature attacks and the opponent has a 2/2 in the block zone.
You will kill their creature and have a 4/2 creature.

# Tosh's Notes
So this game is obviously about anticipation.
There's two things I'm doing to try and make it feel less like guessing though.

Round abilities will help.
You see the player gets a bonus for swinging in on round 3 so you block.
You'll feel good even if really it was the most likely and obvious move.

_I don't know if this will create mindgames that are too frustrating though. Like "ah ha, he things I'm gonna swing with that round 3 creature so I'm going to do something else_

The other thing is splitting turns into rounds.
Having rounds allows players to actually react to what's going on in smaller chunks.
If the game was just turn, draw card, turn, draw card. I think it would feel monotonous.

With rounds the level of anticipation actually closes in near the end of the turn.
So if your opponent has 3 creatures and they've attacked twice, it's not much guesswork what creature they'll attack with.

Blocking might seem a bit... unique, but I think simultaneous combat needs it. 
You could have a system where you pick a creature to block and hope they attack but that seems frustrating.
So you send it to the blockzone and guarantee the block. But that could be strong so the tradeoff is you're stuck blocking everything.

Marking allows you to choose the creature directly, but it's tradeoff is that the opponent now knows about it and you can't do anything else for the round.

The combat flow in general is probably what I want to test the most. Can you have mtg-esque creatures swinging at each other AND a simultaneous combat system?
Who knows? 
Let's find out!

# Resource Ideas - NOTE: This is just a brain dump, only read if you're bored
At first I liked the idea that each player would recieve the same amount of mana but in waves.
So Turn 1 = 1 mana, Turn 2 = 2 Mana but it would tick down eventually.

- Round 1 : 1 Mana
- Round 2 : 2 Mana
- Round 3 : 3 Mana
- Round 4 : 4 Mana
- Round 5 : 3 Mana
- Round 6 : 2 Mana
- Round 7 : 1 Mana
- Round 8 : 2 Mana

I like the idea of playing around the mana cycle, do you go all in on a spike? Conserve when the opponent might run low?
You'd probably need a cap to stop people turtling until mana is no longer an issue.

But from then it go me thinking, it might be cool if people could pick where on the curve to start. 
So a kind of aggro player might want to start curve at 4 mana to play a bunch of creatures.
And the midrange might want.... well actually wouldn't everyone just want to start with the most mana possible?

This lead me to the idea to different mana sources.
So I like the idea of cycles and we've got the sun and moon that do just that.
So how about... Solar Mana and Lunar Mana.
Now you'd have two mana cycles that swing back and forth.
You could even attach times of day. (Dawn, Noon, Midday, Midnight etc)

So now you have 

- ☀️ 4 : 🌙 1
- ☀️ 3 : 🌙 2
- ☀️ 2 : 🌙 3
- ☀️ 1 : 🌙 4
- ☀️ 2 : 🌙 3
- ☀️ 3 : 🌙 2

Now we're kind of getting into actual card identity and deck building restriction stuff right.
I think we can all see the sort of blue/black type cards that would be Lunar cards and the green/white solar.

It would even open up a design space of the idea of any one turn being Lunar dominant or Solar dominant that you could create card effects around.

- ☀️ 1 : 🌙 4 is Lunar dominant
- ☀️ 3 : 🌙 2 is Solar dominant

The only issue with this... it doesn't really restrict deck building, everyone would just use a mix.
You could limit people to one mana source but that seems a bit boring and you'd basically create a "Solar goes first" situation when the game is supposed to be free of traditional turn taking.

So... let's add Earth Mana!

This could be a much more grounded and stable mana source. Like 1-2-2-3-3-2-2-1-2-2 or something.

It would also flavourwise be the domain of the humans and nature and such.

To pull a deckbuilding rule out of my arse, how about you get to pick a max of 2 mana sources.
So your 3 types are

- Light  ☀️/🌎
- Dark   🌙/🌎
- Cosmic 🌙/☀️
- Solar  ☀️
- Earth  🌎
- Lunar  🌙

In order to give picking one mana source any kind of advantage my thought was that the amount of mana you can hold changes based on your choices.

So for example maybe a mono earth can hold up to 6 earth mana, but a Light or Dark deck can only hold up to 4 Earth mana.
You could make cards that cost 5+ Earth mana and know they can only be played in certain decks.

The cycles seem like a lot to hold in your head, but as this is a digital game it frees your mind from having to track it.
You could even have a forecast section of the UI.

**This maybe shit for game design but come on. The day/night cycle flavour is preaty neat right?**

## Really going off the deep end...
So 3 types is kind of... boring.
We're already going with the kind of cosmic theme.
Why not do planets instead! _(as well?)_

So now you could have like 

- Mars    3-3-3-2-2-1-1-2-2 (strong start, tapers out)
- Earth   2-3-3-2-2-3-3-2-2 (grounded, consitent)
- Jupiter 1-2-3-4-3-2-1-2-3 (linear with big tops)
- Neptune 1-1-2-2-3-3-4-4-3 (real slow with big spike)

With something like this, you then would (maybe) get the kind of balance that the colour pie gives us.
You can tap as many sources as you have cards for, but playing a full solar system deck might be kind of awkward.

(Or, more likely the guaranteed resources would make a deck playing all of the planets OP and there's no reason not to do that) 

**This maybe even more shit for game design... THE PLANETORY FLAVOUR THOUGH! The deck building UI could be in a planetarium! The players could be called AstroMages let's go!**

Thanks for listening!
    -Tosh

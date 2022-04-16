# Exercise 1 #

In this exercise you'll configure a Unity scene and write scripts to create an interactive experience. As you progress through the steps, feel free to add comments to the code about *why* you choose to do things a certain way. Add comments if you felt like there's a better, but more time intensive way to implement specific functionality. It's OK to be more verbose in your comments than typical, to give us a better idea of your thoughts when writing the code.

## What you need ##

* Unity 2020 (latest, or whatever you have already)
* IDE of your choice
* Git

## Instructions ##

This test is broken into multiple phases. You can implement one phase at a time or all phases at once, whatever you find to be best for you.

### Phase 1 ###

**Project setup**:

 1. Create a new Unity project inside this directory, put "Virbela" and your name in the project name.
 1. Configure the scene:
     1. Add a central object named "Player"
     1. Add 5 objects named "Item", randomly distributed around the central object
 1. Add two C# scripts named "Player" and "Item" to you project
     1. Attach the scripts to the objects in the scene according to their name, Item script goes on Item objects, Player script goes on Player object.
     1. You may use these scripts or ignore them when pursuing the Functional Goals, the choice is yours. You're free to add any additional scripts you require to meet the functional goals.

**Functional Goal 1**:

When the game is running, make the Item closest to Player turn red. One and only one Item is red at a time. Ensure that when Player is moved around in the scene manually (by dragging the object in the scene view), the closest Item is always red.

### Phase 2 ###

**Project modification**:

 1. Add 5 objects randomly distributed around the central object with the name "Bot"
 1. Add a C# script named "Bot" to your project.
 1. Attach the "Bot" script to the 5 new objects.
     1. Again, you may use this script or ignore it when pursing the Functional Goals.

**Functional Goal 2**:

When the game is running, make the Bot closest to the Player turn blue. One and only one object (Item or Bot) has its color changed at a time. Ensure that when Player is moved around in the scene manually (by dragging the object in the scene view), the closest Item is red or the closest Bot is blue.

### Phase 3 ###

**Functional Goal 3**:

Ensure the scripts can handle any number of Items and Bots.

**Functional Goal 4**:

Allow the designer to choose the base color and highlight color for Items/Bots at edit time.

## Questions ##

 1. How can your implementation be optimized? Rather than check for the closest object every Player update, we could start a coroutine that performs the check every 0.1 seconds which could be sufficient in most cases.  Another option, if we knew the items/bots will never need to move positions in the future, we could make our calls to update colors only when the Player moves rather than when Update is called.
 1. How much time did you spend on your implementation? The initial implementation to get the color switching between different items/bots didn't take too long.  I'd estimate roughly two hours for setting up the project, importing a few starter assets, creating prefabs, and writing the main code to get the colors switching between items and bots.  The majority of my time was spent later on some of the optional extension problems.
 1. What was most challenging for you? The serialization/deserialization took a little bit to get right, especially since the scripts are running in both Edit Mode and Play Mode.  Switching back and forth between Edit and Play Mode was doubling up on certain instantiations since Start() was getting called on the scripts both entering and exiting Play Mode.  I wanted it to work cleanly even though I knew this wouldn't happen once the project was built just in case you all tinker with the project in the Editor.
 1. What else would you add to this exercise? A possible functional goal 5 could be to add random motion to the items/bots so it is more than just the player moving through the scene. Perhaps if the items/bots collide they could destroy each other as well.

## Optional ##

* Add Unit Tests ***Added a sample unit test for the Player script to demonstrate competency with Unity's Unit Test Framework***
* Add XML docs ***Added***
* Optimize finding nearest ***Optimized it fairly well while still maintaining readability***
* Add new Items/Bots automatically on key press ***New items/bots can be added to the scene while in Play Mode by pressing the Enter key or Numpad Enter key.
* Read/write Item/Bot/Player state to a file and restore on launch ***Player/Item/Bot state being restored from save file***

## Next Steps ##

* Confirm you've addressed the functional goals
* Answer the questions above by adding them to this file
* Commit and push the entire repository, with your completed project, back into a repository host of your choice (bitbucket, github, gitlab, etc.)
* Share your project URL with your Virbela contact (Recruiter or Hiring Manager)

## If you have questions ##

* Reach out to your Virbela contact (Recruiter or Hiring Manager)

# [Game Manager System] | Technical Decisions Log
## Game Manager
	Problem: How should a Game Manager be designed?
	Options: 
		- Normal class
		- Singleton
	Decision: Used Singleton pattern
	Reason: To manage game states and global systems with a single access point.

## Reset Game
	Problem: When pressing exit during gameplay, what is the best way to reset it?
	Options: 
		- Create Method and Event to Reset
		- Load new Scene
	Decision: Load new Scene
	Reason: I have multiple coroutine methods that if I need to reset it, it becomes more complicated to write a script for resetting.
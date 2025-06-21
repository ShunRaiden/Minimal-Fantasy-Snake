[Game Manager System] | Technical Decisions Log
1. Game Manager
	Problem: How should a Game Manager be designed?
	Options: 
		- Normal class
		- Singleton
	Decision: Used Singleton pattern
	Reason: To manage game states and global systems with a single access point.
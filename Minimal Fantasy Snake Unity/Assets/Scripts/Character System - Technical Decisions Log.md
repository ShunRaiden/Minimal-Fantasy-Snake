# [Character System] | Technical Decisions Log
## Base Character
	Problem: How should the character control the systems?
	Options: 
		- Put everything in one script
		- Separate parts by function
    Decision: Used Separate parts by function
    Reason: Easier to implement and debug

## Inherite Hero
	Problem: Do Hero have to inherit?
	Options: 
		- Yes
		- No
	Decision: Yes. 
	Reason: Because future Hero will have to moves and more systems.

## Inherite Monster
	Problem: Do Monster have to inherit?
	Options: 
		- Yes
		- No
	Decision: No. 
	Reason: Because at this time there are no plans to add any special systems.

## Character Movement
	Problem: How should characters move on the grid?
	Options: 
		- Direct transform position
		- Rigidbody move position
		- Navmesh Agent
	Decision: Used Navmesh Agent
	Reason: Easier and Fast to use
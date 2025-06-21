# [Game State System] | Technical Decisions Log
## Game State
	Problem: What Pattern I should do with this system?
	Options: 
		- State Pattern
		- Strategy Pattern
		- Observer Pattern
	Decision:  State Pattern
	Reason:	More suitable because
		- Makes it easy to add/remove/modify state without touching the handler code
		- Strategy Pattern is better for Logic
		- Observer Pattern is better for UI

## ICommand
	IEnumerator Execute()

## GameStateManager
	Coroutine currentState

	SetUpState : ICommand		// For Set Up Game
	InputState : ICommand		// For Wait Player Input
	WalkState : ICommand		// For Wait Hero Walk Anim
	CombatState : ICommand		// For Auto Combat
	PostCombatState : ICommand	// For Check More Condition
	GameOverState : ICommand	// For Game Over Step
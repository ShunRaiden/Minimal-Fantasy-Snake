# [UI System] | Technical Decisions Log
## UI Design Pattern
	Problem: How should a UI be designed?
	Options: 
		- UI and Logic together
		- MVC
    Decision: Try to follow the MVC as much as possible.
    Reason: Easy to edit and easy to modify

## UI Rotation Constraint
	Problem: How to keep the UI from changing angles
	Options: 
		- Write code to adjust the angle by yourself
		- Write code using Rotation Constraint
	Decision: Write code using Rotation Constraint
	Reason: Easy to use and Clean
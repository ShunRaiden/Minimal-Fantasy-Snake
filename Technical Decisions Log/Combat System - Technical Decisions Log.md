# [Combat System] | Technical Decisions Log
## Camera Combat Method
	Problem: How should a handler camera object?
	Options: 
		- Have 1 Camera and Moving Camera by 4 Transform
		- Have 4 Camera and Open/Close by Vecter2
    Decision: Have 4 Camera and Open/Close by Vecter2
    Reason: No need to move the camera value frequently, making the camera angle more accurate and the code easier to read.

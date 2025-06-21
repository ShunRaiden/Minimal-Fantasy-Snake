# [Audio System] | Technical Decisions Log
## Audio Manager
	Problem: How should a sound system be designed?
	Options: 
		- Normal class
		- Singleton
    Decision: Used Singleton pattern
    Reason: easy to manages all audio globally.

## Asset Loading Method
	Problem: How should Asset  Loading be designed?
	Options: 
		- Normal loading
		- Addressables
	Decision: Addressables
	Reason: Supports async loading and better memory management.
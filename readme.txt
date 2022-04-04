# Unity Version 2021.2.0f1
 - Import into Unity project
 - Open GrowNCAScene
 - Press Run

List of settings changeable in the editor:

# GameManager
 - Time Scale: Changes how fast the simulation is run. Might have issues if set to over 50
 - Max Iterations: Sets how many times the structures is run through the NCA before it is built
 - Pause: Pauses the simulation to allow for inspection of different structures

# Evaluator
 - Height Weight: How much weight is put into the height evaluation
 - Complexity Weight: How much weight is put into the complexity evaluation
 - Simulation Length: For how long the gravity is simulated on the structure. If too low the evaluation may be false

# Evolution
 - Population: Size of population*
 - Number Of Parents: Number of parents selected at each iteration*
 - Number Of Children: Number of recombinations*
 - Number Of Mutations: Number of mutations*
 - Bias Mutation Proc: How many biases are changed per mutation in percentages
 - Bias Mutation Range: How much each bias can change per mutation. Range is from [-0.25, 0.25]
 - Weight Mutation Proc: How many weights are changed per mutation in percentages
 - Weight Mutation Range: How much each weight can change per mutation. Range is from [-0.25, 0.25]

*If the number of children, mutations and parents is less than population size random neural networks are created until the population size is reached.


Technical Report: Point Update Operations in Fenwick Trees

1. Introduction to Fenwick Tree Point Updates

The objective of a point update in a Fenwick tree is to modify the value at a specific index while maintaining the structural integrity required for prefix sum calculations. In this data structure, point updates serve as the functional counterpart to range queries. While range queries utilize a "cascading down effect" by traversing the tree toward zero to aggregate sums, point updates necessitate an "upward" traversal. This report defines the mechanics of this upward progression and the bitwise logic governing the modification of indices.

2. Comparative Logic: Range Queries vs. Point Updates

The structural navigation of a Fenwick tree depends on the manipulation of the Least Significant Bit (LSB). The direction of traversal is determined by whether the LSB is added or removed.

Operation Type	Bitwise Action	Traversal Direction
Range Queries	Removing the Least Significant Bit (LSB) until reaching zero.	Downward (Toward index 0)
Point Updates	Adding the Least Significant Bit (LSB) until out of bounds.	Upward (Toward index n)

3. The Concept of Cell Responsibility

Updating multiple indices is required by the "range of responsibility" logic fundamental to Fenwick trees. Each cell in the tree array is responsible for a specific range of values. Consequently, when a value at a single index is modified, every cell whose range of responsibility encompasses that index must be updated synchronously.

The source provides a visual metaphor to identify these cells: drawing a line "outwards" or "upwards" from the starting index. The mathematical LSB progression is the computational equivalent of this visual line traversal; any cell "hit" by the line corresponds to an index calculated through the LSB addition process.

4. Procedural Walkthrough: The LSB Addition Process

The point update algorithm identifies the sequence of indices to be modified by iteratively adding the LSB of the current index to itself.

Example 1: Updating Index 9

This progression identifies the cells responsible for index 9 within a given tree structure:

1. Initial Index 9 (binary `1001`): The Least Significant Bit is `1`.
2. First Progression: Add the LSB `1` to `9` to get `10` (binary `1010`).
3. Second Progression: The LSB of `10` (binary `1010`) is `2`. Add `2` to `10` to get `12` (binary `1100`).
4. Third Progression: The LSB of `12` (binary `1100`) is `4`. Add `4` to `12` to get `16` (binary `10000`).
5. Termination: `16` is the final responsible index before the calculation exceeds the array bounds for a tree of this size.

Example 2: Updating Index 6 with Constant X

To add a constant value X at position 6, the algorithm targets the original index and its subsequent upward hops:

1. Initial Index 6: The LSB of 6 is 2. Add 2 to 6 to reach 8.
2. First Progression: The LSB of 8 is 8. Add 8 to 8 to reach 16.

In this scenario, the specific set of indices requiring modification is 6, 8, and 16.

5. Formal Algorithm Definition

The point update is executed via a structured loop that persists as long as the index remains within the defined bounds of the array.

Requirements:

* A Fenwick tree stored in an array of size n.
* An update value X (the constant to be added).
* A starting position i.

Algorithm Logic:

function add(i, x):
    while i < N:
        tree[i] = tree[i] + x
        i = i + LSB(i)

LSB(12) = 4 because 12₁₀ = 1100₂ and the least significant bit of 1100₂ is 100₂, or 4 in base ten

6. Termination and Out-of-Bounds Logic

The algorithm relies on a strict boundary condition: the loop continues only while the current index i is less than the array size n. Once i reaches or exceeds n, the process terminates. This ensures that all responsible cells within the data structure are updated while preventing invalid memory access beyond the array's allocation.

7. Implementation Note: The LSB Function

The "LSB" (Least Significant Bit) function is a prerequisite for navigating the Fenwick tree. This function extracts the value of the lowest set bit to determine the step size for the next upward hop. In professional programming environments, this operation is typically handled by built-in functions provided by the language or hardware, abstracting the underlying bitwise logic from the primary algorithm implementation.

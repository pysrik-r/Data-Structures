# Technical Analysis Report: Linear Probing in Hash Table Open Addressing

*With Integrated Visual Diagrams and Illustrations*

---

## 1. Overview of Open Addressing Foundations

Open addressing is a collision resolution strategy where all key-value pairs are stored directly within the hash table's array, avoiding the overhead of external data structures. The system relies on a "probing function" to resolve collisions by systematically iterating through the table's indices until an available bucket is found.

From an algorithmic perspective, locating an index follows these rigorous procedural steps:

### Algorithm Steps

1. **Initial Hash Calculation**: Determine the "zeroth" position using the base hash of the key: 
   i_0 = {Hash}(key) mod N
   where N is the table size.

2. **Occupancy Evaluation**: Check if $T[i_0]$ is null. If it is empty, the operation (insertion or search) terminates at this index.

3. **Probing Sequence Initialization**: If a collision occurs (index $i_0$ is occupied), initialize a probing variable $x = 1$.

4. **Iterative Probing**:
   - Calculate the next index in the sequence: $i_x = (\text{Hash}(key) + P(x)) \pmod N$
   - $P(x)$ is the probing function that determines the offset
   - Evaluate the bucket at $T[i_x]$

5. **Modification and Loop**: If $T[i_x]$ is occupied by a different key, increment $x$ and repeat Step 4. This "pushes" the search forward until a null slot or the target key is identified.

### Visual Process Flow

```
Linear Probing Process Flow
┌──────────────────────────────────────────────────────────────┐
│ 1. Calculate: i₀ = Hash(key) mod N                          │
├──────────────────────────────────────────────────────────────┤
│ 2. Check if T[i₀] is null?                                   │
│    ├─ YES → Insert here & DONE                               │
│    └─ NO  → Continue to Step 3                               │
├──────────────────────────────────────────────────────────────┤
│ 3. Initialize x = 1 (probe counter)                          │
├──────────────────────────────────────────────────────────────┤
│ 4. Calculate: iₓ = (Hash(key) + P(x)) mod N                 │
│    Where P(x) = ax + b (linear function)                     │
├──────────────────────────────────────────────────────────────┤
│ 5. Is T[iₓ] null?                                            │
│    ├─ YES → Insert here & DONE                               │
│    ├─ NO & key matches → Update value & DONE                 │
│    └─ NO & key differs → x++, repeat Step 4                  │
└──────────────────────────────────────────────────────────────┘
```

---

## 2. Mechanics of the Linear Probing Function

Linear probing defines the search sequence using a linear mathematical formula:

### Formula and Components

* **Formula**: $P(x) = ax + b$
* **Constant $a$**: This multiplier must be non-zero ($a \neq 0$). It functions as the "step size" or "slope" of the probe, determining how far the search moves with each increment of $x$.
* **Constant $b$ (Redundancy)**: In modern implementations, $b$ is considered obsolete. Because the probing sequence begins at an offset already determined by $\text{Hash}(key) \pmod N$, the $b$ parameter merely adds a secondary constant offset to the entire sequence, providing no additional entropy or collision resolution benefit.
* **Linear Execution**: As $x$ increments (1, 2, 3, ...), the algorithm traverses the table in a fixed linear progression, providing high cache locality at the cost of potential clustering.

### Visual Representation of Linear Probing

```
Hash Table State During Insertion
────────────────────────────────────────────────────────────────

Table Size N = 10
Probing Function: P(x) = 1·x (step size = 1)
Load Factor α = 0.4 (4 items in 10 slots)

Index:  0   1   2   3   4   5   6   7   8   9
      ┌───┬───┬───┬───┬───┬───┬───┬───┬───┬───┐
      │K₁ │ ∅ │K₂ │ ∅ │K₃ │ ∅ │ ∅ │K₄ │ ∅ │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┴───┴───┘
       ↓                       ↓
       K₁ hash = 0             K₄ hash = 7
```

### Linear Probing Formula Breakdown

```
P(x) = ax + b
 │    │  │  │
 │    │  │  └─ Constant offset (often b=0 in modern impls)
 │    │  └───── Probe iteration counter (x = 1, 2, 3, ...)
 │    └──────── Step size (must be coprime with N)
 └──────────── Probing function

Example with a=1 (uniform step):
┌────────────┬────────────────────────┬──────────────┐
│ Iteration  │ Calculation            │ Index Result │
├────────────┼────────────────────────┼──────────────┤
│ x = 0      │ (h + 1·0) mod N        │ h            │
│ x = 1      │ (h + 1·1) mod N        │ h + 1        │
│ x = 2      │ (h + 1·2) mod N        │ h + 2        │
│ x = 3      │ (h + 1·3) mod N        │ h + 3        │
└────────────┴────────────────────────┴──────────────┘

Each probe moves forward by exactly 1 position
```

### Working Example: Collision Resolution

```
Problem: Insert key K with Hash(K) = 2 into a full region
Table Size: N = 8
Probing Function: P(x) = x

Initial Table State:
Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │A  │B  │C  │D  │E  │ ∅ │ ∅ │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┘

Insertion Process for Key K (Hash = 2):

STEP 1: Check index 2
  Index 2 contains 'C' (occupied, different key)
  → Continue probing

STEP 2: x=1, Calculate (2 + 1) mod 8 = 3
  Index 3 contains 'D' (occupied, different key)
  → Continue probing

STEP 3: x=2, Calculate (2 + 2) mod 8 = 4
  Index 4 contains 'E' (occupied, different key)
  → Continue probing

STEP 4: x=3, Calculate (2 + 3) mod 8 = 5
  Index 5 is NULL (empty slot found!)
  → INSERT K here

Final Table State:
Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │A  │B  │C  │D  │E  │K  │ ∅ │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┘
       ↑  ↑  ↑  ↑  ↑  ↑
       └──original hash region (clustering)
```

---

## 3. The Infinite Loop Risk and Cycle Prevention

A critical failure state in open addressing occurs when the probing function fails to produce a "full cycle" of order N. If a function only visits a subset of indices, the algorithm may enter an infinite loop during insertion—even if the table has empty slots—if those slots fall outside the specific cycle generated by the function.

### Fundamental Requirement

To guarantee a complete cycle where every bucket in the table is reachable, the step size $a$ and the table size $N$ must be relatively prime. This is defined by the condition:

GCD(a, N) = 1

### Visual Proof: Why GCD(a,N)=1 Matters

```
CRITICAL REQUIREMENT: gcd(a, N) = 1

If gcd(a, N) > 1, the probing function will NOT visit all N indices.

For N = 7 (prime), any a works:
┌──────────────────────────────────────────────────────────┐
│ a=1: 0→1→2→3→4→5→6 (Full cycle, all 7 indices!) ✓      │
│ a=2: 0→2→4→6→1→3→5 (Still full cycle) ✓                │
│ a=3: 0→3→6→2→5→1→4 (Still full cycle) ✓                │
│ gcd(k, 7) = 1 for all k ∈ {1,2,3,4,5,6}                │
└──────────────────────────────────────────────────────────┘

Why? Prime numbers are coprime with all smaller numbers!
```

---

## 4. Practical Case Study I: Cycle Failure Analysis

Consider a configuration where the probing function P(x) = 6x and the table size N = 9.

### GCD Analysis and Failure Prediction

* **GCD Calculation**: $\text{GCD}(6, 9) = 3$. Since the GCD is not 1, the function is mathematically incapable of visiting all 9 buckets.

### Detailed Trace of Insertion Failure for $k_6$ ($\text{Hash}(k_6) = 5$):

```
┌─────────────────────────────────────────────────────────────────┐
│ INSERTION TRACE: P(x) = 6x, N = 9, Hash = 5                   │
├─────────────────────────────────────────────────────────────────┤

PROBE #0 (Initial Check):
  Calculation: (5 + 6·0) mod 9 = 5
  Status: Index 5 (assume occupied or check further)

PROBE #1 (x=1):
  Calculation: (5 + 6·1) mod 9 = 11 mod 9 = 2
  Visited so far: {5, 2}

PROBE #2 (x=2):
  Calculation: (5 + 6·2) mod 9 = 17 mod 9 = 8
  Visited so far: {5, 2, 8}

PROBE #3 (x=3):
  Calculation: (5 + 6·3) mod 9 = 23 mod 9 = 5
  → CYCLE DETECTED! Back to index 5

Resulting Cycle: The sequence {5, 2, 8} repeats indefinitely.
```

### The Partition Problem

```
Hash Table Partitioning with P(x) = 6x, N = 9

Index:  0   1   2   3   4   5   6   7   8
      ┌───┬───┬───┬───┬───┬───┬───┬───┬───┐
      │   │   │   │   │   │   │   │   │   │
      └───┴───┴───┴───┴───┴───┴───┴───┴───┘
       ↑
       Partition 1: {0, 6, 3}
              Partition 2: {1, 7, 4}
                     Partition 3: {2, 8, 5}

Keys are forever locked in their starting partition!
Each partition has exactly 3 slots (33% of table).
Keys cannot escape to reach the other 66% of the table.
```

### Failure Scenario

If these three buckets $\{5, 2, 8\}$ are occupied, the algorithm will loop forever, despite buckets $0, 1, 3, 4, 6,$ and $7$ potentially being empty.

### Resolution Strategy

To satisfy the GCD requirement for $N=9$, the constant $a$ must be chosen from the set $\{1, 2, 4, 5, 7, 8\}$.

```
For N = 9, which values of 'a' satisfy gcd(a, N) = 1?

┌─────────────────────────────────────────────────────────┐
│ VALID STEP SIZES (a) for N = 9                          │
│ a ∈ {1, 2, 4, 5, 7, 8}                                  │
│ All other values will create partial cycles             │
└─────────────────────────────────────────────────────────┘

Each valid 'a' reaches all 9 slots:

a = 1 (Linear): 
  Sequence: 0 → 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 0
  (All 9 indices visited sequentially)

a = 2 (Skip-2): 
  Sequence: 0 → 2 → 4 → 6 → 8 → 1 → 3 → 5 → 7 → 0
  (All 9 indices visited, scrambled order)

a = 4 (Skip-4): 
  Sequence: 0 → 4 → 8 → 3 → 7 → 2 → 6 → 1 → 5 → 0
  (All 9 indices visited, different sequence)
```

---

## 5. Quadratic Probing in Hash Table Collision Resolution

Quadratic probing is an open addressing strategy that uses a second-degree polynomial to compute probe offsets. The general form is:

```
P(x) = ax^2 + bx + c
```

This is only a quadratic probe when `a != 0`. If `a` becomes zero, the formula reduces to a linear probe, which loses the key benefit of quadratic probing and becomes more vulnerable to primary clustering.

### 5.1 Why Quadratic Probing Must Be Chosen Carefully

The essential requirement for any probing function is that it can reach every bucket in the table before repeating. If the probe sequence cycles through only a small subset of indices, insertion may fail even when empty slots exist.

A poor quadratic function can trap a key in a partial cycle.

Example:

* `P(x) = 2x^2 + 2`
* `N = 9`
* initial hash = 4

```
Probe 0: 4
Probe 1: (4 + 2·1^2 + 2) mod 9 = 8
Probe 2: (4 + 2·2^2 + 2) mod 9 = 5
Probe 3: (4 + 2·3^2 + 2) mod 9 = 6
Probe 4: (4 + 2·4^2 + 2) mod 9 = 2
Probe 5: (4 + 2·5^2 + 2) mod 9 = 2
Probe 6: (4 + 2·6^2 + 2) mod 9 = 6
Probe 7: (4 + 2·7^2 + 2) mod 9 = 5
Probe 8: (4 + 2·8^2 + 2) mod 9 = 8
```

This sequence is confined to the indices `{4, 8, 5, 6, 2}`. It never reaches indices `{0, 1, 3, 7}`, so the table is not fully covered. That is the exact risk of an invalid quadratic probe: a non-full cycle and a hidden infinite loop.

### 5.2 Established Quadratic Probing Configurations

The safest quadratic implementations are those with known mathematical coverage guarantees.

Method Number | Probing Function `P(x)` | Table Constraints / Requirements
---|---|---
Method 1 | `P(x) = x^2` | Table size prime `N > 3`; load factor `<= 1/2`
Method 2 | `P(x) = (x^2 + x) / 2` | Table size is a power of 2
Method 3 | alternating sign of `x^2` | Table size is prime with `N ≡ 3 (mod 4)`

### 5.3 Method 2: `P(x) = (x^2 + x) / 2` and Power-of-Two Sizes

This method is especially popular because it guarantees a full cycle when `N` is a power of two. The function is the triangular number formula, and it produces a probe sequence that walks through every index exactly once before repeating.

```
P(x) = (x^2 + x) / 2
```

When the table size is a power of two, the probe offsets form a permutation of the whole table.

#### Insertion Example: Table Size = 8, Load Factor 0.4

Initial state: 8 slots, threshold = 3 elements

1. Insert `K_1` with hash 6
   * initial index 6 → empty → place at 6
2. Insert `K_2` with hash 5
   * initial index 5 → empty → place at 5
3. Insert `K_3` with hash 5
   * initial index 5 → occupied by `K_2`
   * `x = 1`: offset = `(1^2 + 1) / 2 = 1`
     → `(5 + 1) mod 8 = 6` → occupied by `K_1`
   * `x = 2`: offset = `(2^2 + 2) / 2 = 3`
     → `(5 + 3) mod 8 = 0` → empty → place `K_3` at 0

Probe sequence for K_3: 5 → 6 → 0

#### Diagram: Table after 3 insertions

```
Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │K_3│   │   │   │   │K_2│K_1│   │
      └───┴───┴───┴───┴───┴───┴───┴───┘
```

### 5.4 Resizing and Rehashing with Quadratic Probing

When the threshold is reached, the table must expand while preserving the power-of-two property.

* Current size: 8
* Threshold reached at 3 items
* New size: 16
* New threshold: 6

Re-insert items into size 16:

* `K_3` with hash 5 → index 5 free
* `K_2` with hash 5 → collision at 5, `x=1` → index 6 free
* `K_1` with hash 6 → collision at 6, `x=1` → index 7 free

#### Diagram: Table after resizing to 16 and restoring 3 items

```
Index:  0   1   2   3   4   5   6   7   8  ... 15
      ┌───┬───┬───┬───┬───┬───┬───┬───┬───┬─────┐
      │   │   │   │   │   │K_3│K_2│K_1│   │ ... │
      └───┴───┴───┴───┴───┴───┴───┴───┴───┴─────┘
```

### 5.5 Final Operations on the Expanded Table

1. Insert `K_4` with value `35410`
   * hash = `35410 mod 16 = 2`
   * index 2 free → place `K_4`
2. Update `K_3` to `V_5`
   * hash = 5 → index 5 contains `K_3`
   * update in place
3. Insert `K_6` with value `-64013`
   * `-64013 mod 16 = 3`
   * index 3 free → place `K_6`
4. Insert `K_7` with hash 2
   * index 2 occupied by `K_4`
   * `x=1` → offset 1 → index 3 occupied by `K_6`
   * `x=2` → offset 3 → index 5 occupied by `K_3`
   * `x=3` → offset 6 → index 8 free → place `K_7`

#### Resulting indices used by quadratic probing

```
K_7 probe path: 2 → 3 → 5 → 8
```

### 5.6 Implementation Takeaway

Quadratic probing is a powerful collision-resolution strategy, but it is not universal. Its correctness depends on all of the following:

* the chosen polynomial coefficients (`a`, `b`, `c`)
* the table size `N`
* the load factor and resizing policy

When these elements are not carefully matched, a quadratic probe can degrade into a partial cycle or an infinite loop. The most robust practical choice is to pair a known safe quadratic formula with a table size that satisfies its mathematical guarantee, such as using the triangular-number probe for power-of-two table sizes.

---

## 6. Handling Collisions, Updates, and Hash Table Maintenance

The probing sequence is utilized not just for finding empty slots, but for ensuring data integrity during updates.

### Collision Handling Strategy

When a new key hashes to an occupied index, the algorithm follows the probe sequence until the first null slot is found for insertion.

### Update Logic and Key Comparison

This is a critical performance optimization. At every step of the probe sequence, the algorithm must compare the search key with the key existing in the current bucket:

* **Finding a Null Slot**: If the sequence reaches a null value before finding the key, the key does not exist, and the algorithm may proceed with insertion.
* **Finding a Matching Key**: If the keys match, the algorithm updates the existing value rather than continuing the probe. This prevents the insertion of duplicate keys.

### Visual Comparison: Update vs New Insertion Detection

```
Search During Probing

When traversing the probe sequence, at each step:

┌──────────────────────────────────────────────────────┐
│ AT EACH INDEX IN PROBE SEQUENCE:                     │
├──────────────────────────────────────────────────────┤
│                                                      │
│  Is T[index] == NULL?                                │
│  ├─ YES → Key doesn't exist                          │
│  │        → INSERT (or return NOT_FOUND)             │
│  │                                                   │
│  └─ NO → Compare keys:                               │
│         ├─ Keys match?                               │
│         │  ├─ YES → UPDATE existing value            │
│         │  │        OR return FOUND                  │
│         │  │                                         │
│         │  └─ NO → Different key, continue probing   │
│         │          x++, repeat                       │
│         │                                            │
└──────────────────────────────────────────────────────┘

Example Trace:
Searching for K5 (Hash = 2) in table with N=8, P(x)=x

Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │K1 │K2 │K3 │K4 │ ∅ │ ∅ │ ∅ │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┘

x=0: Check index 2 → K3 ≠ K5 → continue
x=1: Check index 3 → K4 ≠ K5 → continue  
x=2: Check index 4 → NULL → Key not found
                    Stop search (K5 doesn't exist)
```

---

## 7. Table Resizing and Data Migration

Hash table efficiency is governed by the Load Factor (alpha). To prevent performance degradation, the table must undergo exponential resizing when a density threshold is met.

### Load Factor and Threshold Calculation

* **Threshold Calculation**: Threshold = N times alpha.
  - Example: For N=12 and alpha=0.35, the threshold is 4. Inserting a 5th element triggers a resize.

### Load Factor Performance Impact

```
RESIZING STRATEGY

Load Factor α = (Number of Items) / (Table Size)

Performance Impact:
┌────────┬──────────────────────────────────┐
│   α    │ Performance Characteristic       │
├────────┼──────────────────────────────────┤
│ 0.25   │ ✓ Excellent (sparse table)      │
│ 0.50   │ ✓ Good (recommended)            │
│ 0.75   │ ⚠ Acceptable (approaching limit)│
│ 0.85   │ ✗ Poor (exponential degradation)│
│ 1.00   │ ✗ CRITICAL (table is full)      │
└────────┴──────────────────────────────────┘

Threshold Calculation:
┌─────────────────────────────────────────┐
│ Threshold = Size × Max α                │
│                                          │
│ Example: N=12, α_max=0.35                │
│ Threshold = 12 × 0.35 = 4 items        │
│                                          │
│ When 5th item inserted → RESIZE!        │
│ New size: 12 × 2 = 24                    │
└─────────────────────────────────────────┘
```

### Exponential Migration Strategy

The table size is typically doubled (e.g., N=12 to N=24). It is imperative that the GCD(a, N_new) = 1 property is maintained after the resize (e.g., if a=5, GCD(5, 24)=1.

### Re-probing Mechanics

Migration requires re-hashing all elements. Because $N$ has changed, the modulo operation yields new indices, and elements that collided in the old table may reside in entirely different relative positions in the new table.

### Migration Walkthrough $(N=12 \to 24, a=5)$

```
BEFORE RESIZE: Table Size = 12, a = 5, 4 keys stored

Index:  0   1   2   3   4   5   6   7   8   9  10  11
      ┌───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┐
      │K₁ │ ∅ │ ∅ │K₃ │ ∅ │ ∅ │K₂ │ ∅ │ ∅ │ ∅ │K₄ │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┘

All 4 keys hash to value 10 (collision scenario)


TRIGGER: Inserting 5th key would exceed α_max = 0.35
ACTION: Resize to 24


AFTER RESIZE: Table Size = 24, a = 5 (still coprime!)

gcd(5, 24) = 1 ✓ Still valid!

New table (initially empty):
Index   0   1   2   3   4   5   6   7   8   9  10  11  12  13  14  15  16  17  18  19  20  21  22  23
      ┌───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┬───┐
      │ ∅ │ ∅ │ ∅ │ ∅ │ ∅ │ ∅ │ ∅ │ ∅ │ ∅ │ ∅ │K₄ │ ∅ │ ∅ │ ∅ │ ∅ │K₃ │ ∅ │ ∅ │ ∅ │ ∅ │K₁ │ ∅ │K₂ │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┴───┘

Re-hashing Process (All keys have same original hash value 10):

K₃ and K₁ both hash to 10 in original table:
  K₃ was at index 3 because: (10 + 5(1)) mod 12 = 3
  K₁ was at index 10 because: initial hash location

In the N=24 table:
  K₃ attempts to occupy index 10. If K₄ occupies 10 first:
    K₃ probes: (10 + 5(1)) mod 24 = 15 ✓ Places K₃ at 15
  
  K₁ attempts index 10 → occupied
    K₁ probes: (10 + 5(1)) mod 24 = 15 → occupied by K₃
    K₁ probes: (10 + 5(2)) mod 24 = 20 ✓ Places K₁ at 20
  
  K₁ now resides at index 20, demonstrating how the change 
  in N forces a complete re-probing of existing data.

RESULT:
  New Load Factor: 4/24 ≈ 0.17 (much lower!)
  New Threshold: 24 × 0.35 = 8.4 items
  Success: Keys redistributed, table ready for more insertions
```

### Resizing Impact Statistics

```
Metric                  Before      After      Change
─────────────────────────────────────────────────────
Table Size              12          24         2×
Items                   4           4          Same
Load Factor α           0.33        0.17       Reduced!
Avg Probe Length        Low         Lower      ✓ Improved
Cache Efficiency        Good        Excellent  ✓ Better

GCD Check:
Before: gcd(5, 12) = 1  ✓
After:  gcd(5, 24) = 1  ✓

Both satisfy the coprimality requirement!
```

---

## 8. Handling Deletions: The Tombstone Problem

One significant limitation of open addressing is the complexity of "Removal" operations. Deleting a key-value pair can create "holes" (null values) in the middle of a probe sequence. Subsequent searches for keys further down that sequence may hit these holes and terminate prematurely, incorrectly reporting that a key does not exist.

### The Problem

```
DELETION CREATES "HOLES"

Consider this table after insertions:

Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │A  │B  │C  │ ∅ │D  │ ∅ │E  │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┘

Key E ended up at index 6 because:
  Hash(E) = 2 → collision at 2,3
  → E was placed at 6 (after skipping 3,4,5)

Now if we DELETE C from index 2:

Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │A  │B  │∅  │ ∅ │D  │ ∅ │E  │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┘

PROBLEM! Search for E:
  x=0: Check index 2 → NULL (STOP: E not found!)
  Never reaches index 6 where E actually is!
```

### The Tombstone Solution

```
SOLUTION: Use Tombstones

Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │A  │B  │⊗  │ ∅ │D  │ ∅ │E  │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┘
            ↑
         Tombstone: "Deleted but skip over"

Search rules:
- NULL: Stop search (key not found)
- TOMBSTONE: Continue probing
- KEY MATCH: Found!

With tombstones, search for E:
  x=0: Index 2 → TOMBSTONE → Continue
  x=1: Index 3 → NULL
  
Or with different probe sequence:
  x=0: Check hash position
  Keep probing through tombstones
  Eventually find E ✓
```

### Insertion with Tombstone Reuse

```
Tombstone State Machine for Insertion

When inserting, treat tombstones differently:

during search:
IF T[index] == TOMBSTONE:
  │
  ├─ Continue probing (might find key further down)
  └─ Remember this position (could reuse for insert)

IF T[index] == NULL:
  │
  ├─ If we saw tombstones earlier: reuse that slot
  └─ Otherwise: insert here

Example:
Index:  0   1   2   3   4   5   6   7
      ┌───┬───┬───┬───┬───┬───┬───┬───┐
      │A  │B  │⊗₁ │⊗₂ │D  │ ∅ │E  │ ∅ │
      └───┴───┴───┴───┴───┴───┴───┴───┘

Inserting new key F (Hash = 2):
  x=0: Index 2 = TOMBSTONE → continue, remember position 2
  x=1: Index 3 = TOMBSTONE → continue
  x=2: Index 4 = KEY D (different) → continue
  x=3: Index 5 = NULL → stop
  
  Insert at: position 2 (reuse first tombstone)
```

---

## 8. Summary of Technical Constraints

Linear probing remains a standard because of its simplicity and efficiency, particularly when $P(x) = 1 \cdot x$. Since $\text{GCD}(1, N)$ is always 1, this configuration guarantees a full cycle regardless of table size.

### Why Linear Probing is Preferred

- **Simplicity**: Easy to implement and understand
- **Cache Locality**: All data in contiguous memory (excellent for modern CPUs)
- **Performance**: Fast for low load factors (excellent average case)
- **Deterministic**: Behavior is predictable and reproducible

### Key Implementation Considerations

```
BEST PRACTICES CHECKLIST

┌─────────────────────────────────────────────────────────┐
│ STEP 1: Choose Table Size                               │
│ ─────────────────────────────────────────────────────   │
│ ☐ Prefer prime numbers (all values coprime)             │
│ ☐ Or use power of 2 (choose a carefully)                │
│ ☐ Initial size: 2^4 = 16 (balance)                      │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ STEP 2: Select Step Size 'a'                            │
│ ─────────────────────────────────────────────────────   │
│ ☐ Verify gcd(a, N) = 1 ALWAYS                           │
│ ☐ Most reliable: a = 1 (simple linear)                  │
│ ☐ For prime N: any a ∈ [1, N-1] works                   │
│ ☐ Document any non-trivial choices                      │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ STEP 3: Set Load Factor Threshold                       │
│ ─────────────────────────────────────────────────────   │
│ ☐ Default: α_max = 0.35                                 │
│ ☐ Aggressive: α_max = 0.50                              │
│ ☐ Never exceed 0.75 (performance cliff)                 │
│ ☐ Threshold = N × α_max                                 │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ STEP 4: Implement Resize                                │
│ ─────────────────────────────────────────────────────   │
│ ☐ Double table size: N_new = 2 × N_old                  │
│ ☐ RE-VERIFY gcd: gcd(a, N_new) must = 1                 │
│ ☐ Rehash ALL elements (modulo changes!)                 │
│ ☐ For prime strategy: choose new prime > 2N_old         │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│ STEP 5: Handle Deletions                                │
│ ─────────────────────────────────────────────────────   │
│ ☐ Use tombstones (⊗) instead of truly null              │
│ ☐ Search through tombstones (don't stop at ⊗)           │
│ ☐ Insertion can reuse tombstone positions               │
│ ☐ Periodic cleanup: rebuild table without tombstones    │
└─────────────────────────────────────────────────────────┘
```

---

## 9. Key Parameter Summary

| Parameter | Description | Requirement / Technical Note |
|-----------|-------------|------------------------------|
| $P(x)$ | Probing Function | Defined linearly as $ax + b$ |
| $N$ | Table Size | Critical denominator in the index modulo operation |
| $a$ | Step Size (Slope) | Must satisfy $\text{GCD}(a, N) = 1$ for a complete cycle |
| $\alpha$ | Max Load Factor | Usually kept low (e.g., 0.35–0.5) to minimize probe lengths |
| Threshold | Resize Trigger | Calculated as $N \times \alpha$ |
| $x$ | Probe Increment | Incremented iteratively starting at 1 upon collision |

### Parameter Relationship Diagram

```
┌─────────────────┬──────────────────────────────────┬─────────────────────────┐
│ Parameter       │ Description                      │ Technical Requirement   │
├─────────────────┼──────────────────────────────────┼─────────────────────────┤
│ P(x)            │ Probing Function                 │ P(x) = ax + b           │
│                 │ Determines search order          │ (b often omitted)       │
├─────────────────┼──────────────────────────────────┼─────────────────────────┤
│ N               │ Table Size (capacity)            │ Must be positive        │
│                 │ Denominator in modulo operation  │ Often prime or 2^k      │
├─────────────────┼──────────────────────────────────┼─────────────────────────┤
│ a (step size)   │ Multiplier in P(x) = ax + b     │ gcd(a, N) = 1           │
│                 │ Distance between probes          │ (CRITICAL!)             │
├─────────────────┼──────────────────────────────────┼─────────────────────────┤
│ H(key)          │ Initial hash value               │ 0 ≤ H(key) < N          │
│                 │ Starting point before probing    │ Uniform distribution    │
├─────────────────┼──────────────────────────────────┼─────────────────────────┤
│ α (load factor) │ Items / Table Size               │ α < 0.5 (recommended)   │
│                 │ Density metric                   │ α < 0.8 (maximum)       │
├─────────────────┼──────────────────────────────────┼─────────────────────────┤
│ Threshold       │ When to trigger resize           │ Threshold = N × α_max   │
│                 │ Default: 0.35-0.50              │ Triggers resize 2N      │
├─────────────────┼──────────────────────────────────┼─────────────────────────┤
│ x               │ Probe iteration counter          │ x = 0, 1, 2, ...        │
│                 │ Incremented on each collision    │ x < N (loop prevention) │
└─────────────────┴──────────────────────────────────┴─────────────────────────┘
```

---

## 10. Quick Troubleshooting: Infinite Loop Prevention

```
DEBUGGING CHECKLIST FOR INFINITE LOOPS

Issue: Insertion never completes (infinite loop)

┌─────────────────────────────────────┐
│ CHECK 1: Verify gcd(a, N) = 1       │
├─────────────────────────────────────┤
│ Most common cause!                  │
│                                     │
│ Example:                            │
│ N = 12 (composite)                  │
│ a = 4 → gcd(4,12) = 4 ✗             │
│ a = 5 → gcd(5,12) = 1 ✓             │
│                                     │
│ Fix: Choose coprime 'a'             │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ CHECK 2: Is table actually full?    │
├─────────────────────────────────────┤
│ If α = 1.0 (100% occupied), normal  │
│ Even with gcd=1, can't find space   │
│ Solution: Trigger resize earlier    │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ CHECK 3: Verify hash calculation    │
├─────────────────────────────────────┤
│ Ensure: 0 ≤ H(key) < N              │
│ Handle negative hash values         │
│ Check: H(key) % N (not % someOther) │
└─────────────────────────────────────┘

┌─────────────────────────────────────┐
│ CHECK 4: Confirm modulo operations  │
├─────────────────────────────────────┤
│ Index = (H(key) + P(x)) mod N       │
│ Not:    (H(key) + P(x)) mod N_old   │
│ Not:    H(key) + (P(x) mod N)       │
└─────────────────────────────────────┘
```

---

## 11. Performance Analysis Summary

### Operation Complexity

```
OPERATION COMPLEXITY

┌────────────┬──────────────┬──────────────┬─────────────────┐
│ Operation  │ Best Case    │ Average Case │ Worst Case      │
│            │ (α low)      │ (α = 0.5)    │ (α high)        │
├────────────┼──────────────┼──────────────┼─────────────────┤
│ Search     │ O(1)         │ O(1 + ε)     │ O(n)            │
│ Insert     │ O(1)         │ O(1 + ε)     │ O(n)            │
│ Delete     │ O(1)         │ O(1 + ε)     │ O(n) + rebuild  │
│ Resize     │ N/A          │ O(n)         │ O(n)            │
└────────────┴──────────────┴──────────────┴─────────────────┘
```

### Probe Length vs Load Factor

```
┌──────────┬──────────────────┬──────────────────┬──────────────┐
│    α     │ Linear Probing   │ Quadratic Prob.  │ Double Hash  │
│ (density)│ Avg Probes       │ Avg Probes       │ Avg Probes   │
├──────────┼──────────────────┼──────────────────┼──────────────┤
│   0.10   │       1.06       │       1.05       │     1.05     │
│   0.25   │       1.17       │       1.15       │     1.14     │
│   0.50   │       1.50       │       1.44       │     1.39     │
│   0.75   │       3.00       │       2.01       │     1.85     │
│   0.90   │      10.0        │       5.11       │     2.56     │
│   0.95   │      20.0        │      10.58       │     3.15     │
│   0.99   │     100.0        │      67.7        │     5.43     │
└──────────┴──────────────────┴──────────────────┴──────────────┘

Key Insight: Linear probing degrades RAPIDLY as α approaches 1.0
             This is why resizing is critical!
```

---

## Final Thoughts

Linear probing is a fundamental technique in hash table implementation because it combines theoretical soundness with practical efficiency. The critical insight is maintaining the $\text{GCD}(a, N) = 1$ invariant before implementation and after resizing operations. This single mathematical constraint prevents the catastrophic failure mode of infinite loops and guarantees that the probing algorithm can always find an empty slot (if one exists) before the table is completely full.

For practitioners implementing hash tables with linear probing:

1. **Always verify the GCD constraint** before deploying the system
2. **Keep load factors well below 0.5** to maintain O(1) average performance
3. **Handle resizing carefully** to preserve the coprimality requirement
4. **Use tombstones for deletions** to maintain search correctness
5. **Consider prime table sizes** for maximum flexibility with step sizes

This disciplined approach transforms linear probing from a simple concept into a robust, production-ready collision resolution strategy.


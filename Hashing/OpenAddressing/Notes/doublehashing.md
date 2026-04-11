# Double Hashing in Open Addressing

## 1. What Is Double Hashing?

Double hashing is a way to fix collisions in hash tables that use open addressing.

- First, a key `k` is mapped to a position using a main hash function `H_1(k)`.
- If that position is empty, the key is stored there.
- If the position is already taken, we use another hash function `H_2(k)` to jump to a new place.

So instead of checking the next slot in a fixed order, double hashing jumps by a value that depends on the key itself.

## 2. How the Probe Works

Imagine the hash table has `N` slots.

- Initial slot: `index = H_1(k) mod N`
- If that slot is full, try:
  - `index = (H_1(k) + 1 * H_2(k)) mod N`
  - `index = (H_1(k) + 2 * H_2(k)) mod N`
  - `index = (H_1(k) + 3 * H_2(k)) mod N`

Each try keeps the same starting point but moves further by the key-dependent step size `H_2(k)`.

### Simple idea

- `H_1(k)` gives a starting position.
- `H_2(k)` gives a step size.
- `x` is how many steps we have taken so far.

That means the probe formula is:

`index = (H_1(k) + x * H_2(k)) mod N`

## 3. Why Use a Second Hash Function?

The second hash `H_2(k)` makes the search path different for different keys.

- If two keys start at the same place, they will move by different steps.
- That reduces the chance that they keep colliding on the same sequence of slots.

This is better than linear probing, where every collision moves by `+1`.

## 4. Keeping the Step Size Safe

We must avoid a bad step size.

- Compute `Δ = H_2(k) mod N`.
- If `Δ == 0`, set `Δ = 1`.

Why? Because a step size of `0` means we would try the same slot again and again.

## 5. Why the Table Should Be Prime

A prime-sized table helps double hashing visit every slot.

If `N` is prime and `Δ` is between `1` and `N-1`, then the sequence of slots will eventually reach every slot in the table.

That means we will not get stuck cycling through only a few positions.

## 6. Simple Example

Use a table of size `N = 7`.

We insert five keys: `K1`, `K2`, `K3`, `K4`, `K6`.

| Key | H_1(k) | Start | H_2(k) | Step `Δ` | Result |
|-----|--------|-------|--------|----------|--------|
| K1  | 67     | 4     | 34     | 6        | slot 4 |
| K2  | 2      | 2     | -79    | 5        | slot 2 |
| K3  | 2      | 2     | 10     | 3        | slot 5 |
| K4  | 2      | 2     | 7      | 0 -> 1   | slot 3 |
| K6  | 3      | 3     | 23     | 2        | slot 0 |

### Step-by-step

- `K1` goes to slot `4` because it is empty.
- `K2` goes to slot `2` because it is empty.
- `K3` starts at slot `2`, but `K2` is there. Then it moves by `3` to slot `5`.
- `K4` starts at slot `2` too. Its step size is `0`, so we change it to `1`. Then it moves to slot `3`.
- `K6` starts at slot `3`, but now `K4` is there. It moves by `2` to slot `5`, which is also full. Then it moves by `2` again to slot `0` and stores there.

## 7. The Resize Rule

When the table fills up too much, we make it bigger.

- Double the size and then pick the next prime number.
- Reinsert every key using the new table size.

Example:

- old size `7` -> double to `14` -> next prime is `17`
- `H_1(k)` and `H_2(k)` do not change
- but the final index changes because `mod N` uses the new `N`

## 8. Easy Summary

- Double hashing starts at `H_1(k) mod N`.
- If the slot is full, it jumps by `H_2(k)` each time.
- `H_2(k)` is changed to `Δ = H_2(k) mod N`.
- If `Δ == 0`, use `1` instead.
- A prime table size helps reach every slot.
- When the table grows, reinsert all keys with the new size.

## 9. Visual Diagram

```mermaid
flowchart TB
  Start[Start insert key k] --> Hash1[Compute H_1(k)]
  Hash1 --> StartIndex[Start index = H_1(k) mod N]
  StartIndex --> Check{Slot empty?}
  Check -- Yes --> Insert[Insert key]
  Check -- No --> Hash2[Compute H_2(k)]
  Hash2 --> Delta[Δ = H_2(k) mod N]
  Delta --> ZeroCheck{Δ == 0?}
  ZeroCheck -- Yes --> Fix[Set Δ = 1]
  ZeroCheck -- No --> Keep[Keep Δ]
  Fix --> Probe
  Keep --> Probe
  Probe[Compute new index = (H_1(k) + x*Δ) mod N]
  Probe --> Next[Try again with x+1]
  Next --> Check
```
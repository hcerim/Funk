# Reading notes

## Functional Programming in C#

Functional programming (**FP**) gives you:

- **Power** - Get more done with less code. Every line is a liability and asset. By reducing code size you are having less liability by keeping the assets.
- **Safety** - Stronger typing and declarative style of writing code. This is especially beneficial when dealing with concurrency due to function purity and state immutability.
- **Clarity** - Code is more readable and understandable. This is especially important during consumption and maintenance of the existing code.

FP emphasizes functions while avoiding state mutation. Functions are first-class values. C# has functions as first-class values, e.g.

```
Func<int, int> triple = x => x * 3; // function as a variable
var range = Enumerable.Range(1, 3);
var triples = range.Select(triple); // x => x * 3 instead of triple

Output:
3,6,9
```

FP rules are when the object is created it should never change again and the variable should never be reassigned.

Mutation (**in-place** value change) example:

```
int[] nums = { 1, 2, 3 };
nums[0] = 7; // update

// Also called destructive update as the value stored prior to the update is destroyed.
```

Mutation (in-place update) -> sorting example:

```
var original = new List<int> { 5, 7, 1 };
original.Sort();

// original is now 1, 5, 7 where the original order is destroyed and lost forever. (This Sort() method is introduced before LINQ and the change in to functional direction)
```

Pure FP languages don't allow in-place updates.

Following this principle, sorting or filtering should never update existing list but create a new one without affecting the old one, e.g.

```
Func<int, bool> isOdd = x => x % 2 == 1;
int[] original = { 7, 6, 1 };
var sorted = original.OrderBy(x => x);
var filtered = original.Where(isOdd);  // x => x % 2 == 1 instead of isOdd
```

Parallel execution example:

```
var nums = Range(-10000, 10000).Reverse().ToList();
Action task1 = () => WriteLine(nums.Sum());
Action task2 = () => { nums.Sort(); WriteLine(nums.Sum()); };
Parallel.Invoke(task1, task2);

// some wrong result
// 0 -> correct
```

`Sort()` as mentioned is not a pure function and it mutates the state of the nums which is used by `task1` hence the wrong computation.

Using `LINQ`'s `OrderBy(x => x)` will give correct result as it is a pure function that returns a new collection.

**C#'s garbage collection makes a programming model that avoids in-place updates possible.**

However, by default everything in C# is mutable and the only way to disable mutation is by marking something as `readonly`. This is opposite to what F# has. You have to mark something mutable to enable its mutation.

Usually, C# developers work with sequences (`IEnumerable`) in a right (functional) way and with everything else in an imperative way. This is due to the lack of understanding of the design principles behind `LINQ`.

C# has get-only auto properties -> complier implicitly declares readonly backing field.

```
public string Name { get; } // value can only be assigned in constructor.

public class Person
{
    public Person(string name) => Name = name;
}
```

After construction of `Person` object, the object can never change, therefore we can say it is immutable.


# Recognizers.NET

This is a simple combinator library for recognizing strings using only imperative constructs. It
features:

 1. no heap allocation
 2. no higher-order functions
 3. no virtual dispatching
 4. at least as powerful as regular expressions
 5. efficient variable capture

This makes the library incredibly efficient. The API is also suitable
for use in low-level imperative languages, like C, which either lack the above features,
or are intended to be used without them.

# Atoms

Some simple "atom" recognizers provided by the library, ie. recognizers for basic types:

    // recognize one digit
    public static bool Digit(ref this Input x, ref Position pos, string capture = null) =>
        pos.Pos < x.Length && char.IsNumber(x[pos]) && pos.Advance();

    // recognize many digits
    public static bool Digits(ref this Input x, ref Position pos)
    {
        var i = pos;
        while (i.Pos < x.Length && char.IsNumber(x[i]))
            ++i.Pos;
        return pos.AdvanceTo(i);
    }

    // recognize many whitespace characters
    public static bool WhiteSpaces(ref this Input x, ref Position pos)
    {
        var i = pos;
        while (i.Pos < x.Length && char.IsWhiteSpace(x[i]))
            ++i.Pos;
        return pos.AdvanceTo(i);
    }

# Combinators

You can combine these simple recognizers using the ordinary boolean operators, && and ||,
where && will succeed if both recognizers match, and || will succeed if either recognizer
matches:

    // recognize strings like "(1234)"
    public static bool BracketedDigits(this ref Input x, ref Position pos) =>
        pos.Save(out var i) && x.Char('(', ref i) && x.Digits(ref i) && x.Char(')', ref i) && pos.AdvanceTo(i);

    // matches one character in the range [A-z] or [0-9]
    public static bool LetterOrDigit(ref this Input x, ref Position pos) =>
        x.Letter(ref pos) || x.Digit(ref pos);

Save/AdvanceTo perform the backtracking that's needed in case the rule fails.
Recognizer combinations can get quite sophisticated. Here's one for phone numbers:

    public static bool PhoneNumber(this ref Input x, ref Position pos) =>
           pos.Save(out var i)
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.Char('+', ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.Digits(ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.BracketedDigits(ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.Optional(x.Char('/', ref i))
        && x.Optional(x.WhiteSpaces(ref i))
        && x.DelimitedDigits(ref i, ' ', '-')
        && pos.AdvanceTo(i);

You can see that this is essentially a specification of what a phone number should
look like, and it's executable to recognize and extract phone numbers!

By convention, methods in plural form are greedy and will consume as many matching
characters as possible, where singular form matches only a single instance.

I'm not sure if this API has ever been done before, but it's kind of neat that you can
almost get to parser combinators without any dynamic allocation or higher-order
functions. It only requires a little discipline and repetition in some cases,
particularly around recursive combinators.

Finally, this API isn't just suitable for strings, but can be written to operate
on bits, bytes or tokens. This could make for some easy to read, but fast
binary pattern matching.

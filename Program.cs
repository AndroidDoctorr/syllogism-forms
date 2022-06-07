Console.WriteLine($"Mood  |  1  |  2  |  3  |  4  |");
foreach (Form majForm in Enum.GetValues<Form>())
{
    foreach (Form minForm in Enum.GetValues<Form>())
    {
        foreach (Form conForm in Enum.GetValues<Form>())
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{majForm}{minForm}{conForm}: ");
            // Determine validity of mood for each figure
            Validity fig1Valid = GoFigure(1, majForm, minForm, conForm);
            DisplayValidity(fig1Valid);
            Validity fig2Valid = GoFigure(2, majForm, minForm, conForm);
            DisplayValidity(fig2Valid);
            Validity fig3Valid = GoFigure(3, majForm, minForm, conForm);
            DisplayValidity(fig3Valid);
            Validity fig4Valid = GoFigure(4, majForm, minForm, conForm);
            DisplayValidity(fig4Valid);
            Console.Write("\n");
        }
    }
}

Validity GoFigure(int fig, Form majForm, Form minForm, Form conForm)
{
    // Venn diagram regions work like this:
    // 3 circles that intersect, S, M, and P, which creates 7 regions
    // 0 - The center, where all intersect
    // 1 - The intersection of S and P
    // 2 - The intersection of S and M
    // 3 - The intersection of M and P
    // 4 - S alone
    // 5 - M alone
    // 6 - P alone
    Existence[] regions = new Existence[7];
    // If both premises are particular, return invalid
    if ((minForm == Form.I || minForm == Form.O) &&
        (majForm == Form.I || majForm == Form.O))
    {
        return Validity.Invalid;
    }
    // If both premises are negative, return invalid
    if ((minForm == Form.E || minForm == Form.O) &&
        (majForm == Form.E || majForm == Form.O))
    {
        return Validity.Invalid;
    }
    // if minor is extreme (E, A)
    if (minForm == Form.A || minForm == Form.E)
    {
        ApplyPremise(regions, fig, minForm, false);
        ApplyPremise(regions, fig, majForm, true);
        // DisplayRegions(regions);
    }
    else
    {
        ApplyPremise(regions, fig, majForm, true);
        ApplyPremise(regions, fig, minForm, false);
        // DisplayRegions(regions);
    }
    // DisplayRegions(regions);
    // Determine if the applied premises produce a conclusion of the given form
    return IsValid(regions, conForm);
}

void ApplyPremise(Existence[] regions, int fig, Form form, bool isMajor)
{
    TwoVenn venn;
    if (fig == 1)
    {
        venn = isMajor ?
            SetTerms(regions, Term.M, Term.P) :
            SetTerms(regions, Term.S, Term.M);
    }
    else if (fig == 2)
    {
        venn = isMajor ?
            SetTerms(regions, Term.P, Term.M) :
            SetTerms(regions, Term.S, Term.M);
    }
    else if (fig == 3)
    {
        venn = isMajor ?
            SetTerms(regions, Term.M, Term.P) :
            SetTerms(regions, Term.M, Term.S);
    }
    else
    {
        venn = isMajor ?
            SetTerms(regions, Term.P, Term.M) :
            SetTerms(regions, Term.M, Term.S);
    }

    ApplyForm(regions, venn, form);
}

TwoVenn SetTerms(Existence[] regions, Term leftTerm, Term rightTerm)
{
    TwoVenn twoVenn = new TwoVenn();
    if (leftTerm == Term.S && rightTerm == Term.P)
    {
        twoVenn.Left = new int[] { 2, 4 };
        twoVenn.Middle = new int[] { 0, 1 };
        twoVenn.Right = new int[] { 3, 6 };
    }
    else if (leftTerm == Term.S && rightTerm == Term.M)
    {
        twoVenn.Left = new int[] { 1, 4 };
        twoVenn.Middle = new int[] { 0, 2 };
        twoVenn.Right = new int[] { 3, 5 };
    }
    else if (leftTerm == Term.M && rightTerm == Term.S)
    {
        twoVenn.Left = new int[] { 3, 5 };
        twoVenn.Middle = new int[] { 0, 2 };
        twoVenn.Right = new int[] { 1, 4 };
    }
    else if (leftTerm == Term.M && rightTerm == Term.P)
    {
        twoVenn.Left = new int[] { 2, 5 };
        twoVenn.Middle = new int[] { 0, 3 };
        twoVenn.Right = new int[] { 1, 6 };
    }
    else if (leftTerm == Term.P && rightTerm == Term.S)
    {
        twoVenn.Left = new int[] { 3, 6 };
        twoVenn.Middle = new int[] { 0, 1 };
        twoVenn.Right = new int[] { 2, 4 };
    }
    else if (leftTerm == Term.P && rightTerm == Term.M)
    {
        twoVenn.Left = new int[] { 1, 6 };
        twoVenn.Middle = new int[] { 0, 3 };
        twoVenn.Right = new int[] { 2, 5 };
    }
    return twoVenn;
}

void ApplyForm(Existence[] regions, TwoVenn venn, Form form)
{
    if (venn == null) return;
    if (venn.Left == null || venn.Right == null || venn.Middle == null) return;
    // E/0 - Universal Negative - No S are P
    if (form == Form.E)
    {
        FillRegions(regions, venn.Middle, new int[] { });
    }
    // I/0.25 - Particualr Affirmative - Some S are P
    else if (form == Form.I)
    {
        FillRegions(regions, new int[] { }, venn.Middle);
    }
    // O/0.75 - Particular Negative - Some S are not P
    else if (form == Form.O)
    {
        FillRegions(regions, new int[] { }, venn.Left);
    }
    // A/1 - Universal Affirmative - All S are P
    else if (form == Form.A)
    {
        FillRegions(regions, venn.Left, new int[] { });
    }
}

void FillRegions(Existence[] regions, int[] no, int[] yes)
{
    foreach (int i in no)
        regions[i] = Existence.No;
    foreach (int i in yes)
    {
        if (regions[i] == Existence.No) continue;
        regions[i] = Existence.Yes;
    }
}

Validity IsValid(Existence[] regions, Form form)
{
    // Regardless of figure, S and P have the same regions
    // Region 5 is irrelevant

    // A/1 - Universal Affirmative - All S are P
    if (form == Form.A)
    {
        // 2 and 4 must be No, 0 and 1 must not both be No
        if (BothAreNo(regions, 2, 4) && BothAreNotNo(regions, 0, 1))
            return Validity.Valid;
    }
    // E/0 - Universal Negative - No S are P
    else if (form == Form.E)
    {
        // 0 and 1 must be No, 2 and 4 must not both be No
        if (BothAreNo(regions, 0, 1) && BothAreNotNo(regions, 2, 4))
            return Validity.Valid;
    }
    // I/0.25 - Particualr Affirmative - Some S are P
    else if (form == Form.I)
    {
        // Unconditionally valid
        if (BothAreNotNo(regions, 2, 4) && BothAreNotNo(regions, 3, 6) && AtLeastOneIsYes(regions, 0, 1))
            return Validity.Valid;
        // Conditionally valid
        if (regions[1] == Existence.No && regions[2] == Existence.No)
            return Validity.IfS;
        if (regions[2] == Existence.No && regions[3] == Existence.No)
            return Validity.IfM;
        if (regions[1] == Existence.No && regions[3] == Existence.No)
            return Validity.IfP;
    }
    // O/0.75 - Particular Negative - Some S are not P
    else if (form == Form.O)
    {
        // Unconditionally valid
        if (BothAreNotNo(regions, 0, 1) && BothAreNotNo(regions, 3, 6) && AtLeastOneIsYes(regions, 2, 4))
            return Validity.Valid;
        if (BothAreNo(regions, 0, 2) && BothAreNo(regions, 1, 6))
            return Validity.IfS;
        if (BothAreNo(regions, 0, 3))
            return Validity.IfM;
    }
    return Validity.Invalid;
}

bool BothAreNo(Existence[] regions, int A, int B)
{
    return regions[A] == Existence.No && regions[B] == Existence.No;
}

bool BothAreNotNo(Existence[] regions, int A, int B)
{
    return !BothAreNo(regions, A, B);
}

bool AtLeastOneIsYes(Existence[] regions, int A, int B)
{
    return regions[A] == Existence.Yes || regions[B] == Existence.Yes;
}

void DisplayValidity(Validity validity)
{
    Console.ForegroundColor = ConsoleColor.White;
    if (validity == Validity.Valid)
    {
        Console.BackgroundColor = ConsoleColor.Green;
        Console.Write(" Yes ");
    }
    else if (validity == Validity.Invalid)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.Write(" No  ");
    }
    else
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        if (validity == Validity.IfS)
        {
            Console.Write("=> ≡S");
        }
        else if (validity == Validity.IfM)
        {
            Console.Write("=> ≡M");
        }
        else if (validity == Validity.IfP)
        {
            Console.Write("=> ≡P");
        }
    }
    Console.ResetColor();
}

/*
void DisplayRegions(Existence[] regions)
{
    foreach (Existence ex in regions)
    {
        Console.Write(ex + ", ");
    }
    Console.Write("\n");
}
*/

/*
void DisplayRegions(Existence[] regions)
{
    for (int i = 0; i < regions.Length; i++)
    {
        Existence ex = regions[i];
        if (ex == Existence.Yes)
        {
            Console.Write(i);
        }
    }
    Console.Write(",");
}
*/

// Univ aff, Univ neg, part aff, part neg
public enum Form { A, E, I, O };
// public enum Form { I, E, A, O, U };
public enum Term { S, M, P };
public enum Existence { Unknown = 0, No = -1, Yes = 1 };
public enum Validity { Invalid, Valid, IfS, IfP, IfM };
class TwoVenn
{
    public int[]? Left;
    public int[]? Right;
    public int[]? Middle;
}
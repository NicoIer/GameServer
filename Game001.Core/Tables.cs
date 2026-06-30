namespace cfg;

public partial class Tables
{
    public static readonly Tables current;

    static Tables()
    {
        current = new Tables();
    }
}
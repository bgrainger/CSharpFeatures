namespace CSharp10;

// "record struct" can be used to declare a record value type
record struct Point2D(int X, int Y);

// "readonly record struct", analogous to "readonly struct"
readonly record struct Point3D(int X, int Y, int Z);

// can now write "record class" instead of "record" for clarity if desired
record class RecordClass(string Name, int Age);

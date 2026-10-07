@interface Tag {
    String value();
}

@Tag("first")
record First(int x) {}

@Tag(value = "second")
record Second(int x, int... rest) {}

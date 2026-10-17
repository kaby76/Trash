int f() {
    int values[2] = {1, 2};
    for (int i = 0; i < 2; i++) {
        values[i] = values[i] + 1;
    }
    return values[0];
}

public static class IntExtension {

    public static bool IsWithinRange(this int number, float minimum, float maximum) {
        return number >= minimum && number < maximum;
    }

    public static int RoundClamp(this int number, int minimum, int maximum) {
        if (number < minimum) {
            return maximum - minimum + number + 1;
        } else if (number > maximum) {
            return number - maximum + minimum - 1;
        }

        return number;
    }
}

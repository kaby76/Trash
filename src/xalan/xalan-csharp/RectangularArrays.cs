//----------------------------------------------------------------------------------------
//	Copyright © 2007 - 2017 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class includes methods to convert Java rectangular arrays (jagged arrays
//	with inner arrays of the same length).
//----------------------------------------------------------------------------------------
internal static class RectangularArrays
{
    internal static int[][] ReturnRectangularIntArray(int size1, int size2)
    {
        int[][] newArray = new int[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new int[size2];
        }

        return newArray;
    }

    internal static sbyte[][] ReturnRectangularSbyteArray(int size1, int size2)
    {
        sbyte[][] newArray = new sbyte[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new sbyte[size2];
        }

        return newArray;
    }

    internal static char[][] ReturnRectangularCharArray(int size1, int size2)
    {
        char[][] newArray = new char[size1][];
        for (int array1 = 0; array1 < size1; array1++)
        {
            newArray[array1] = new char[size2];
        }

        return newArray;
    }
}
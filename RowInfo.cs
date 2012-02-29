using System;
// info about each row/column of the matrix for simplification purposes
class RowInfo
{
	internal const int ROW_NORMAL = 0; // ordinary value
	internal const int ROW_CONST = 1; // value is constant
	internal const int ROW_EQUAL = 2; // value is equal to another value
	internal int nodeEq, type, mapCol, mapRow;
	internal double value_Renamed;
	internal bool rsChanges; // row's right side changes
	internal bool lsChanges; // row's left side changes
	internal bool dropRow; // row is not needed in matrix
	internal RowInfo()
	{
		type = ROW_NORMAL;
	}
}
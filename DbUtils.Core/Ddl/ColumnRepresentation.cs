using System;

namespace DbUtils.Core
{
	public class ColumnRepresentation
	{
		
		public ColumnRepresentation ()
		{
		}

		public string Name {
			get;
			set;
		}

		public string DataType {
			get;
			set;
		}

		public bool PrimaryKey {
			get;
			set;
		}

		public bool NotNull {
			get;
			set;
		}

		public int Size {
			get;
			set;
		}
	}
}


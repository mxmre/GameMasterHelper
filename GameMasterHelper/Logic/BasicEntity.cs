using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMasterHelper.Logic
{
	[Serializable]
    public class BasicEntity : ICloneable
    {
		public BasicEntity()
		{
			Name = "null";
			Description = "null";
		}

		public BasicEntity(string name, string description)
        {
            this.p_name = name;
            this.p_description = description;
        }

        private string p_name;

		public string Name
		{
			get { return p_name; }
			set { p_name = value; }
		}

		private string p_description;

		public string Description
        {
			get { return p_description; }
			set { p_description = value; }
		}
		public bool IsEmpty()
		{
			return p_name == "null";
		}

        public object Clone()
        {
            return new BasicEntity(this.Name.Clone() as string,
				this.Description.Clone() as string);
        }
    }

}

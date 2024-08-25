using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameMasterHelper.Logic
{
    [Serializable]
    public class Range<T>
		where T : IComparable<T>
    {
		private void PropertyInitCheck()
		{
			if (p_min.CompareTo(p_max) > 0)
				throw new ArgumentException("min > max");
				
		}
        public Range() : this(default(T), default(T))
        {
        }
        public Range(T min, T max) 
		{
			p_min = min; p_max = max;
			this.PropertyInitCheck();
		}

        private T p_min;

		public T Minimum
		{
			get { return p_min; }
			set 
			{ 
				p_min = value;
                this.PropertyInitCheck();
            }
		}

        private T p_max;

		public T Maximum
        {
			get { return p_max; }
			set 
			{ 
				p_max = value;
                this.PropertyInitCheck();
            }
		}

	}
    [Serializable]
    public class Slider<T> : Range<T>
        where T : IComparable<T>
    {
        private void SliderPropertyUpdate()
        {
            if (p_slider.CompareTo(Maximum) > 0)
                p_slider = Maximum;
            else if (p_slider.CompareTo(Minimum) < 0)
                p_slider = Minimum;
        }
        public Slider() : this(default(T), default(T), default(T))
        { }
        public Slider(T min, T slider, T max) : base(min, max)
        {
			p_slider = slider;
			this.SliderPropertyUpdate();
        }

		private T p_slider;

		public T SliderValue
		{
			get { return p_slider; }
			set 
			{ 
				p_slider = value;
                this.SliderPropertyUpdate();
			}
		}

        public new T Minimum
        {
            get { return base.Minimum; }
            set
            {
                base.Minimum = value;
                if (p_slider.CompareTo(base.Minimum) < 0)
                    p_slider = base.Minimum;
            }
        }

        public new T Maximum
        {
            get { return base.Maximum; }
            set
            {
                base.Maximum = value;
                if (p_slider.CompareTo(base.Maximum) > 0)
                    p_slider = base.Maximum;
            }
        }

    }
}

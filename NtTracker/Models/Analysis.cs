using System.ComponentModel.DataAnnotations;

namespace NtTracker.Models
{
    public class Analysis
    {
        ///
        /// Properties
        ///

        [ScaffoldColumn(false)]
        [Key]
        public int Id { get; set; }

        public double? Hemoglobin { get; set; }

        public double? Hematocrit { get; set; }

        public double? PlateletCount { get; set; }

        public double? Alt { get; set; }

        public double? Ast { get; set; }

        public double? Cpk { get; set; }

        public double? Proteins { get; set; }

        public double? Sodium { get; set; }

        public double? Potassium { get; set; }

        public double? Chloride { get; set; }

        ///
        /// Methods
        ///

        /// <summary>
        /// Checks if a value is in the normal range.
        /// </summary>
        /// <param name="propertyIndex">Index of the property to check.</param>
        /// <param name="value">Property value.</param>
        /// <returns>bool indicating if the value is in range.</returns>
        public bool CheckRange(int propertyIndex, double? value)
        {
            var res = true;

            if (value == null) return true;

            switch (propertyIndex)
            {
                //Hemoglobin
                case 0:
                    res = value > 140 && value < 200;
                    break;

                //Hematocrit
                case 1:
                    res = value > 0.31 && value < 0.63;
                    break;

                //PlateletCount
                case 2:
                    res = value > 100000 && value < 350000;
                    break;

                //Alt
                case 3:
                    res = value > 4.8 && value < 21;
                    break;

                //Ast
                case 4:
                    res = value > 6 && value < 60;
                    break;

                //Cpk
                case 5:
                    res = value > 9.6 && value < 320;
                    break;

                //Proteins
                case 6:
                    res = value > 60 && value < 84;
                    break;

                //Sodium
                case 7:
                    res = value > 134 && value < 147;
                    break;

                //Potassium
                case 8:
                    res = value > 3.5 && value < 5.9;
                    break;

                //Chloride
                case 9:
                    res = value > 95 && value < 110;
                    break;
            }

            return res;
        }

        /// <summary>
        /// Returns the computed score (out of a max of 10) of this Analysis.
        /// </summary>
        /// <returns></returns>
        public int ComputeScore()
        {
            var score = 10;

            if (!CheckRange(0, Hemoglobin)) score--;
            if (!CheckRange(1, Hematocrit)) score--;
            if (!CheckRange(2, PlateletCount)) score--;
            if (!CheckRange(3, Alt)) score--;
            if (!CheckRange(4, Ast)) score--;
            if (!CheckRange(5, Cpk)) score--;
            if (!CheckRange(6, Proteins)) score--;
            if (!CheckRange(7, Sodium)) score--;
            if (!CheckRange(8, Potassium)) score--;
            if (!CheckRange(9, Chloride)) score--;

            return score;
        }
    }
}
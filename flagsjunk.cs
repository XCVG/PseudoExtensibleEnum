
        //these won't work as-is: changing type to target could scramble bits, and we need a way to compare bitwise when types are not known
        /*
        public static bool HasFlagUnchecked(Enum flag, object value)
        {
            if (value == null)
                throw new ArgumentNullException();

            var valueType = value.GetType();
            var underlyingType = valueType.IsEnum ? Enum.GetUnderlyingType(valueType) : valueType;

            object valueValue = Convert.ChangeType(value, underlyingType);

            object flagValue = Convert.ChangeType(Convert.ChangeType(flag, Enum.GetUnderlyingType(flag.GetType())), underlyingType);

            return valueValue & flagValue == flagValue;
        }
        */

        /*
        public static bool HasFlag(Enum flag, object value)
        {
            if (value == null)
                throw new ArgumentNullException();

            var valueType = value.GetType();
            var underlyingType = valueType.IsEnum ? Enum.GetUnderlyingType(valueType) : valueType;

            object valueValue = Convert.ChangeType(value, underlyingType);

            object flagValue = Convert.ChangeType(Convert.ChangeType(flag, Enum.GetUnderlyingType(flag.GetType())), underlyingType);

            if(valueType.IsEnum && !IsDefined(valueType, flag))
            {
                throw new ArgumentException($"flag '{flag}' is not defined in base or extended type for enum {valueType.Name}");
            }

            return valueValue & flagValue == flagValue;
        }
        */
        
        private ulong ConvertBitsToLong(object value)
        {

        }
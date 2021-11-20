using CCore.Net.JsRt;
using System;

namespace CCore.Net.Managed
{
    public class JsNumber : JsValue
    {
        public static new bool isSupported(JsValueType type, JsValueRef value) => type == JsValueType.Number;

        public static JsValueRef FromNumber(object value)
        {
            TypeCode typeCode = Type.GetTypeCode(value.GetType());
            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return JsValueRef.From(Convert.ToInt32(value));
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return JsValueRef.From(Convert.ToDouble(value));
            }
            throw new Exception("Invalid value type, exptected a number.");
        }

        public JsNumber(int value) : this(JsValueRef.From(value))
        {
        }

        public JsNumber(double value) : this(JsValueRef.From(value))
        {
        }

        public JsNumber(JsValueRef jsValue)
        {
            if (!jsValue.IsValid)
                throw new Exception("Invalid value");
            if (!isSupported(jsValue.ValueType, jsValue))
                throw new Exception("Unsupported type.");

            jsValueRef = jsValue;
        }

        public static implicit operator int(JsNumber number) => number.jsValueRef.ToInt32();
        public static implicit operator long(JsNumber number) => Convert.ToInt64(number.jsValueRef.ToInt32());
        public static implicit operator short(JsNumber number) => Convert.ToInt16(number.jsValueRef.ToInt32());
        public static implicit operator byte(JsNumber number) => Convert.ToByte(number.jsValueRef.ToInt32());
        public static implicit operator double(JsNumber number) => number.jsValueRef.ToDouble();
        public static implicit operator decimal(JsNumber number) => Convert.ToDecimal(number.jsValueRef.ToDouble());
        public static implicit operator float(JsNumber number) => Convert.ToSingle(number.jsValueRef.ToDouble());

        public static explicit operator JsNumber(int num) => new JsNumber(num);
        public static explicit operator JsNumber(double num) => new JsNumber(num);
    }
}

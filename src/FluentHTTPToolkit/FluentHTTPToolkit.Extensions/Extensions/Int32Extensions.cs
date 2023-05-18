namespace FluentHTTPToolkit.Extensions
{
    public static class Int32Extensions
    {
        /// <summary> 
        /// Определяет, находится ли заданное число в интервале между двумя другими числами.
        /// </summary>
        /// <param name="num">Число для проверки</param> 
        /// <param name="lower">Нижняя граница интервала</param> 
        /// <param name="upper">Верхняя граница интервала</param> 
        /// <param name="inclusive"> 
        /// Если true, то метод вернет true, если число равно одной из границ интервала,
        /// иначе - только если оно строго больше нижней и меньше верхней границы. 
        /// </param> 
        /// <returns>
        /// True, если число находится в указанном диапазоне,
        /// Иначе false
        /// </returns>
        public static bool Between(this int num, int lower, int upper, bool inclusive = true)
        {
            return inclusive
                ? lower <= num && num <= upper
                : lower < num && num < upper;
        }
    }
}

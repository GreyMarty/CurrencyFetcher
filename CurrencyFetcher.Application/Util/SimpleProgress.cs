namespace CurrencyFetcher.Application.Util
{
    public struct SimpleProgress
    {
        public float CurrentValue { get; set; }
        public float TargetValue { get; set; }

        public SimpleProgress(float currentValue, float targetValue)
        {
            CurrentValue = currentValue;
            TargetValue = targetValue;
        }
    }
}

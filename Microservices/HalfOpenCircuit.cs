using Polly;
using Polly.CircuitBreaker;

namespace ContactKeeper.Microservices
{
    public class HalfOpenCircuit
    {
        private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;

        public HalfOpenCircuit()
        {
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 1,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, duration) =>
                    {
                        Console.WriteLine($"Circuit broken due to: {exception.Message}. Will retry in {duration.TotalSeconds} seconds.");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("Circuit reset. Ready to try again.");
                    });
        }
        public async Task ExecuteAsync(Func<Task> action)
        {
            await _circuitBreakerPolicy.ExecuteAsync(action);
        }
    }
}
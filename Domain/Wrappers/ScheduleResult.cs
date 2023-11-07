namespace Domain.Wrappers
{
    public class ScheduleResult<T> : Result
    {
        public ScheduleResult()
        {
        }
        public ScheduleResult(List<T> data)
        {
            Data = data;
        }

        public List<T> Data { get; set; }

        internal ScheduleResult(bool succeeded, List<T> data = default, List<string> messages = null)
        {
            Data = data;
            Succeeded = succeeded;
        }
        public new static ScheduleResult<T> Fail()
        {
            return new ScheduleResult<T> { Succeeded = false };
        }

        public new static ScheduleResult<T> Fail(string message)
        {
            return new ScheduleResult<T> { Succeeded = false, Messages = new List<string> { message } };
        }

        public new static ScheduleResult<T> Fail(List<string> messages)
        {
            return new ScheduleResult<T> { Succeeded = false, Messages = messages };
        }
        public new static ScheduleResult<T> Fail(List<T> data, string message)
        {
            return new ScheduleResult<T> { Succeeded = false, Data = data, Messages = new List<string> { message } };
        }
        public new static ScheduleResult<T> Fail(List<T> data, List<string> messages)
        {
            return new ScheduleResult<T> { Succeeded = false, Data = data, Messages = messages };
        }

        public new static Task<ScheduleResult<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public new static Task<ScheduleResult<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public new static Task<ScheduleResult<T>> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }
        public new static Task<ScheduleResult<T>>FailAsync(List<T> data, string message)
        {
            return Task.FromResult(Fail(data, message));
        }
        public  static Task<ScheduleResult<T>> FailAsync(List<T> data, List<string> messages)
        {
            return Task.FromResult(Fail(data, messages));
        }
        public new static ScheduleResult<T> Success()
        {
            return new ScheduleResult<T> { Succeeded = true };
        }

        public new static ScheduleResult<T> Success(string message)
        {
            return new ScheduleResult<T> { Succeeded = true, Messages = new List<string> { message } };
        }

        public static ScheduleResult<T> Success(List<T> data)
        {
            return new ScheduleResult<T> { Succeeded = true, Data = data };
        }

        public static ScheduleResult<T> Success(List<T> data, string message)
        {
            return new ScheduleResult<T> { Succeeded = true, Data = data, Messages = new List<string> { message } };
        }

        public static ScheduleResult<T> Success(List<T> data, List<string> messages)
        {
            return new ScheduleResult<T> { Succeeded = true, Data = data, Messages = messages };
        }

        public new static Task<ScheduleResult<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public new static Task<ScheduleResult<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<ScheduleResult<T>> SuccessAsync(List<T> data)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<ScheduleResult<T>> SuccessAsync(List<T> data, string message)
        {
            return Task.FromResult(Success(data, message));
        }
        public static Task<ScheduleResult<T>> SuccessAsync(List<T> data, List<string> messages)
        {
            return Task.FromResult(Success(data, messages));
        }
    }
}

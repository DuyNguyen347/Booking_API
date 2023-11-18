using Application.Dtos.Requests;
using Application.Dtos.Responses;
using Application.Interfaces.Services;
using Domain.Entities.Seat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class SeatReservationService : ISeatReservationService
    {
        private Dictionary<long, Dictionary<int, SeatLockInfo>> reservation;
        private int timeOutInSecond;
        private readonly object lockObject = new object();
        public SeatReservationService() 
        {
            reservation = new Dictionary<long, Dictionary<int, SeatLockInfo>>();
            timeOutInSecond = 300;
        }
        public Dictionary<long, Dictionary<int, AddSeatReservationResponse>> GetAll()
        {
            Dictionary<long, Dictionary<int, AddSeatReservationResponse>> LockSeats = new Dictionary<long, Dictionary<int, AddSeatReservationResponse>>();
            foreach(var (scheduleId, seat) in reservation)
            {
                LockSeats[scheduleId] = new Dictionary<int, AddSeatReservationResponse>();
                foreach (var (numberSeat, info) in seat)
                {
                    LockSeats[scheduleId][numberSeat] = new AddSeatReservationResponse()
                    {
                        LockTime = info.LockTime,
                        LockBy = info.LockBy
                    };
                }
            }
            return LockSeats;
        }
        public bool LockSeats(long customerId, long scheduleId, List<int> numberSeats)
        {
            lock (lockObject)
            {
                foreach (var seat in numberSeats)
                {
                    if (IsSeatLocked(scheduleId, seat))
                    {
                        return false;
                    }
                }
                foreach (var seat in numberSeats)
                {
                    LockSeat(customerId, scheduleId, seat, DateTime.Now);
                }
                return true;
            }
        }
        public void UnlockSeats(long customerId, long scheduleId, List<int> numberSeats)
        {
            foreach (var Seat in numberSeats)
            {
                if (ValidateLock(customerId, scheduleId, Seat))
                {
                    UnlockSeat(scheduleId, Seat);
                }
            }
        }
        public HashSet<int> GetLockedSeats(long scheduleId)
        {
            HashSet<int> lockedSeats = new HashSet<int>();
            if (reservation.ContainsKey(scheduleId))
            {
                foreach (var (numberSeat, seatLockInfo) in reservation[scheduleId])
                {
                    if (!seatLockInfo.IsExpired())
                        lockedSeats.Add(numberSeat);
                    else
                        UnlockSeat(scheduleId, numberSeat);
                }
            }
            return lockedSeats;
        }
        private bool IsSeatLocked(long scheduleId, int numberSeat)
        {
            if (!reservation.ContainsKey(scheduleId)) return false;
            if (!reservation[scheduleId].ContainsKey(numberSeat)) return false;
            if (reservation[scheduleId][numberSeat].IsExpired()) return false;
            return true;
        }  
        private void LockSeat(long customerId, long scheduleId, int numberSeat, DateTime lockTime)
        {
            if (!reservation.ContainsKey(scheduleId))
            {
                reservation[scheduleId] = new Dictionary<int, SeatLockInfo>();
            }
            if (!reservation[scheduleId].ContainsKey(numberSeat))
            {
                reservation[scheduleId][numberSeat] = new SeatLockInfo()
                {
                    TimeOutInSecond = timeOutInSecond
                };
            }
            reservation[scheduleId][numberSeat].LockTime = lockTime;
            reservation[scheduleId][numberSeat].LockBy = customerId;
        }
        public bool ValidateLock(long customerId, long scheduleId, int numberSeat)
        {
            return IsSeatLocked(scheduleId, numberSeat) && reservation[scheduleId][numberSeat].LockBy == customerId;
        }
        private void UnlockSeat(long scheduleId, int numberSeat)
        {
            if (!reservation.ContainsKey(scheduleId)) return;
            reservation[scheduleId].Remove(numberSeat);
        }
    }
    public class SeatLockInfo
    {
        public DateTime LockTime { get; set; }
        public long LockBy { get; set; }
        public int TimeOutInSecond {  get; set; }
        public bool IsExpired()
        {
            return LockTime.AddSeconds(TimeOutInSecond) < DateTime.Now;
        }
    }
}

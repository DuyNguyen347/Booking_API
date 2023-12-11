using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Category;
using Application.Interfaces.CategoryFilm;
using Application.Interfaces.Cinema;
using Application.Interfaces.Customer;
using Application.Interfaces.Employee;
using Application.Interfaces.EnumMasterData;
using Application.Interfaces.Feedback;
using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Film;
using Application.Interfaces.FilmImage;
using Application.Interfaces.Reply;
using Application.Interfaces.Schedule;
using Application.Interfaces.Room;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Application.Interfaces.View.ViewCustomerBookingHistory;
using Application.Interfaces.View.ViewCustomerFeedbackReply;
using Application.Interfaces.View.ViewCustomerReviewHistory;
using Application.Interfaces.WorkShift;
using Infrastructure.Repositories.Booking;
using Infrastructure.Repositories.BookingDetail;
using Infrastructure.Repositories.Category;
using Infrastructure.Repositories.CategoryFilm;
using Infrastructure.Repositories.Cinema;
using Infrastructure.Repositories.Customer;
using Infrastructure.Repositories.Employee;
using Infrastructure.Repositories.EnumMasterData;
using Infrastructure.Repositories.Feedback;
using Infrastructure.Repositories.FeedbackFileUpload;
using Infrastructure.Repositories.Film;
using Infrastructure.Repositories.FilmImage;
using Infrastructure.Repositories.Reply;
using Infrastructure.Repositories.Schedule;
using Infrastructure.Repositories.Room;
using Infrastructure.Repositories.Service;
using Infrastructure.Repositories.ServiceImage;
using Infrastructure.Repositories.View.ViewCustomerBookingHistory;
using Infrastructure.Repositories.View.ViewCustomerFeedbackReply;
using Infrastructure.Repositories.View.ViewCustomerReviewHistory;
using Infrastructure.Repositories.WorkShift;
using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Ticket;
using Infrastructure.Repositories.Ticket;
using Infrastructure.Repositories.Seat;
using Application.Interfaces.Seat;
using Application.Interfaces.Merchant;
using Infrastructure.Repositories.Merchant;
using Application.Interfaces.Review;
using Infrastructure.Repositories.Review;

namespace Infrastructure.Extensions
{
    public static class AddRepositoryExtensions
    {
        public static void AddEmployeeRepository(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }
        public static void AddServiceRepository(this IServiceCollection services)
        {
            services.AddScoped<IServiceRepository, ServiceRepository>();
        }
        public static void AddServiceImageRepository(this IServiceCollection services)
        {
            services.AddScoped<IServiceImageRepository, ServiceImageRepository>();
        }
        public static void AddCustomerRepository(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }
        public static void AddBookingRepository(this IServiceCollection services)
        {
            services.AddScoped<IBookingRepository,BookingRepository>();
        }
        public static void AddCategoryRepository(this IServiceCollection services)
        {
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }

        public static void AddCinemaRepository(this IServiceCollection services)
        {
            services.AddScoped<ICinemaRepository, CinemaRepository>();
        }

        public static void AddRoomRepository(this IServiceCollection services)
        {
            services.AddScoped<IRoomRepository, RoomRepository>();
        }

        public static void AddFilmRepository(this IServiceCollection services)
        {
            services.AddScoped<IFilmRepository, FilmRepository>();
        }
        public static void AddCategoryFilmRepository(this IServiceCollection services)
        {
            services.AddScoped<ICategoryFilmRepository, CategoryFilmRepository>();
        }
        public static void AddFilmImageRepository(this IServiceCollection services)
        {
            services.AddScoped<IFilmImageRepository, FilmImageRepository>();
        }

        public static void AddSeatRepository(this IServiceCollection services)
        {
            services.AddScoped<ISeatRepository, SeatRepository>();
        }

        public static void AddBookingDetailRepository(this IServiceCollection services)
        {
            services.AddScoped<IBookingDetailRepository, BookingDetailRepository>();
        }
        public static void AddViewCustomerBookingHistoryRepository(this IServiceCollection services)
        {
            services.AddScoped<IViewCustomerBookingHistoryRepository,ViewCustomerBookingHistoryRepository>();
        }
        public static void AddFeedbackRepository(this IServiceCollection services)
        {
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        }
        public static void AddReplyRepository(this IServiceCollection services)
        {
            services.AddScoped<IReplyRepository,ReplyRepository>();
        }
        public static void AddWorkShiftRepository(this IServiceCollection services)
        {
            services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();
        }
        public static void AddViewCustomerFeedbackReplyRepository(this IServiceCollection services)
        {
            services.AddScoped<IViewCustomerFeedbackReplyRepository,ViewCustomerFeedbackReplyRepostiory>();
        }
        public static void AddViewCustomerReviewHistoryRepository(this IServiceCollection services)
        {
            services.AddScoped<IViewCustomerReviewHisotyRepository,ViewCustomerReviewHistoryRepository>();
        }
        public static void AddFeedbackFileUploadRepository(this IServiceCollection services)
        {
            services.AddScoped<IFeedbackFileUploadRepository, FeedbackFileUploadRepository>();
        }
        public static void AddEnumMasterDataRepository(this IServiceCollection services)
        {
            services.AddScoped<IEnumMasterDataRepository, EnumMasterDataRepository>();
        }
        public static void AddScheduleRepository (this IServiceCollection services)
        {
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
        }
        public static void AddTicketRepository(this IServiceCollection services)
        {
            services.AddScoped<ITicketRepository, TicketRepository>();
        }
        public static void AddMerchantRepository(this IServiceCollection services)
        {
            services.AddScoped<IMerchantRepository, MerchantRepository>();
        }
        public static void AddReviewRepository(this IServiceCollection services)
        {
            services.AddScoped<IReviewRepository, ReviewRepository>();
        }
    }
}

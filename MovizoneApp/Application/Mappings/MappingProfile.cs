using AutoMapper;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for mapping between Domain models and DTOs
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Movie mappings
            CreateMap<Movie, MovieDto>();
            CreateMap<CreateMovieDto, Movie>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate ?? DateTime.UtcNow));
            CreateMap<UpdateMovieDto, Movie>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ReleaseDate, opt => opt.MapFrom(src => src.ReleaseDate ?? DateTime.UtcNow));

            // TVSeries mappings
            CreateMap<TVSeries, TVSeriesDto>();
            CreateMap<CreateTVSeriesDto, TVSeries>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Episodes, opt => opt.Ignore());
            CreateMap<UpdateTVSeriesDto, TVSeries>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Episodes, opt => opt.Ignore());

            // Actor mappings
            CreateMap<Actor, ActorDto>();
            CreateMap<CreateActorDto, Actor>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate ?? DateTime.UtcNow.AddYears(-30)));
            CreateMap<UpdateActorDto, Actor>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate ?? DateTime.UtcNow.AddYears(-30)));

            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password will be hashed separately
            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.MovieTitle, opt => opt.Ignore()) // Will be populated from Movie
                .ForMember(dest => dest.UserName, opt => opt.Ignore()); // Will be populated from User
            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
            CreateMap<UpdateReviewDto, Review>()
                .ForMember(dest => dest.MovieId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Watchlist mappings
            CreateMap<WatchlistItem, WatchlistDto>()
                .ForMember(dest => dest.MovieTitle, opt => opt.Ignore()) // Will be populated from Movie
                .ForMember(dest => dest.MovieCoverImage, opt => opt.Ignore())
                .ForMember(dest => dest.MovieGenre, opt => opt.Ignore())
                .ForMember(dest => dest.MovieRating, opt => opt.Ignore());
            CreateMap<CreateWatchlistItemDto, WatchlistItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AddedAt, opt => opt.Ignore());

            // PricingPlan mappings
            CreateMap<PricingPlan, PricingPlanDto>();
            CreateMap<CreatePricingPlanDto, PricingPlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdatePricingPlanDto, PricingPlan>();
        }
    }
}

using AutoMapper;
using QuizGame.DataAccess.Models;
using QuizGame.Shared.Models;

namespace QuizGame.WebAPI.Infrastructure;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateQuizDto, Quiz>(MemberList.Source)
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.PinCode, opt => opt.MapFrom(_ => GeneratePin()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => QuizState.Draft))
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
        //
        CreateMap<UpdateQuizDto, Quiz>(MemberList.Source)
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.PinCode, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));

        CreateMap<UpdateQuestionDto, Question>(MemberList.Source)
            .ForMember(dest => dest.AnswerOptions, opt => opt.MapFrom(src => src.Answers));
        
        CreateMap<UpdateAnswerOptionDto, AnswerOption>(MemberList.Source);
        //

        CreateMap<Quiz, QuizListItemDto>(MemberList.Destination)
            .ForMember(
                dest => dest.HasActiveSession,
                opt => opt.MapFrom(src =>
                    src.Sessions.Any(s =>
                        s.State != QuizSessionState.Finished))
            )
            .ForMember(
                dest => dest.ActiveSessionId,
                opt => opt.MapFrom(src =>
                    src.Sessions
                        .Where(s => s.State != QuizSessionState.Finished)
                        .Select(s => (int?)s.Id)
                        .FirstOrDefault())
            );

        CreateMap<Quiz, QuizDetailsDto>(MemberList.Destination)
            .ForMember(
                dest => dest.Questions,
                opt => opt.MapFrom(src => src.Questions))
            .ForMember(
                dest => dest.HasActiveSession,
                opt => opt.MapFrom(src =>
                    src.Sessions.Any(s =>
                        s.State != QuizSessionState.Finished))
            )
            .ForMember(
                dest => dest.ActiveSessionId,
                opt => opt.MapFrom(src =>
                    src.Sessions
                        .Where(s => s.State != QuizSessionState.Finished)
                        .Select(s => (int?)s.Id)
                        .FirstOrDefault())
            );

        CreateMap<CreateQuestionDto, Question>(MemberList.Source)
            .ForMember(dest => dest.AnswerOptions, opt => opt.MapFrom(src => src.Answers));

        CreateMap<Question, QuestionDto>(MemberList.Destination)
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.AnswerOptions));

        CreateMap<CreateAnswerOptionDto, AnswerOption>(MemberList.Source);

        CreateMap<AnswerOption, AnswerOptionDto>(MemberList.Destination);

        CreateMap<Question, ActiveQuestionDto>(MemberList.Destination)
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.AnswerOptions));

        CreateMap<QuizSession, QuizSessionDto>(MemberList.Destination)
            .ForMember(dest => dest.SessionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Quiz.Title))
            .ForMember(dest => dest.IsStarted, opt => opt.MapFrom(src => src.State == QuizSessionState.InProgress))
            .ForMember(dest => dest.IsFinished, opt => opt.MapFrom(src => src.State == QuizSessionState.Finished))
            .ForMember(dest => dest.ParticipantsCount, opt => opt.MapFrom(src => src.Participants.Count))
            .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsQuestionLocked))
            .ForMember(dest => dest.CurrentQuestion, opt => opt.Ignore())
            .ForMember(dest => dest.IsOwner, opt => opt.Ignore())
            .ForMember(dest => dest.ParticipantId, opt => opt.Ignore())
            .ForMember(dest => dest.HasAnswered, opt => opt.Ignore())
            .ForMember(dest => dest.SelectedAnswerId, opt => opt.Ignore())
            .ForMember(dest => dest.IsCorrect, opt => opt.Ignore())
            .ForMember(dest => dest.ParticipantNickname, opt => opt.Ignore());

        CreateMap<UserRequestDto, User>(MemberList.Source)
            .ForSourceMember(src => src.Password, opt => opt.DoNotValidate())
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<User, UserResponseDto>(MemberList.Destination);
    }

    private static string GeneratePin()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}
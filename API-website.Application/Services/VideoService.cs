using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using API_website.Core.Models;
using Xabe.FFmpeg;
using System.IO;
using System.Globalization;
public class VideoService
{
    private readonly IVideoRepository _videoRepository; // переименовали поле
    private readonly IUserRepository _userRepository;
    public VideoService(IVideoRepository videoRepository, IUserRepository userRepository)
    {
        _videoRepository = videoRepository; // инициализация с корректным именем
        _userRepository = userRepository;
    }
    public async Task<string> TrimVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // Получаем путь к видео
        string pathVideo = _videoRepository.GetPathVideo(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        var startTime = optionsFfmpeg.StartTime;
        var endTime = optionsFfmpeg.EndTime;

        if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
        {
            throw new ArgumentException("Start time and end time must be provided.");
        }

        // Проверяем, что файл существует
        if (!File.Exists(pathVideo))
        {
            throw new FileNotFoundException($"The input video file '{pathVideo}' does not exist.");
        }

        // Формируем путь для временного файла
        var directory = Path.GetDirectoryName(pathVideo);
        if (directory == null) throw new FileNotFoundException($"Not directory");
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        try
        {
            // Настраиваем FFmpeg
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{pathVideo}\"")  // Входной файл
                .AddParameter($"-ss {startTime}")    // Время начала
                .AddParameter($"-to {endTime}")      // Время окончания
                .AddParameter("-c copy")            // Копирование без перекодирования
                .SetOutput(tempFilePath)            // Временный файл для обработки
                .SetOverwriteOutput(true);          // Разрешаем перезапись

            // Асинхронно запускаем конвертацию
            await conversion.Start();

            // Удаляем оригинальный файл
            if (File.Exists(pathVideo))
            {
                File.Delete(pathVideo);
            }

            // Переименовываем временный файл в оригинальное имя
            File.Move(tempFilePath, pathVideo);

            return pathVideo; // Возвращаем путь к обновлённому файлу
        }
        catch (Exception ex)
        {
            // Удаляем временный файл в случае ошибки
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            throw new InvalidOperationException("Failed to trim the video.", ex);
        }
    }

    // Основной метод с полной функциональностью

    public async Task<string> ReverseVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // Получаем путь к видео
        string pathVideo = _videoRepository.GetPathVideo(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        if (string.IsNullOrEmpty(pathVideo) || !File.Exists(pathVideo)) 
            throw new FileNotFoundException($"The input video file '{pathVideo}' does not exist.");

        // Формируем путь для временного файла
        var directory = Path.GetDirectoryName(pathVideo);
        if (directory == null) throw new FileNotFoundException($"Not directory");
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        try
        {
            // Настраиваем FFmpeg для реверса видео и аудио
            var conversion = FFmpeg.Conversions.New()
                .AddParameter($"-i \"{pathVideo}\"")    // Входной файл
                .AddParameter("-vf reverse")           // Реверс видео
                .AddParameter("-af areverse")          // Реверс аудио
                .SetOutput(tempFilePath)               // Временный выходной файл
                .SetOverwriteOutput(true);             // Разрешаем перезапись

            // Асинхронно запускаем конвертацию
            await conversion.Start();

            // Удаляем оригинальный файл
            if (File.Exists(pathVideo))
            {
                File.Delete(pathVideo);
            }

            // Переименовываем временный файл в оригинальное имя
            File.Move(tempFilePath, pathVideo);

            return pathVideo; // Возвращаем путь к обновлённому файлу
        }
        catch (Exception ex)
        {
            // Удаляем временный файл в случае ошибки
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            throw new InvalidOperationException("Failed to reverse the video.", ex);
        }
    }



    public async Task<string> SpeedVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // Получаем путь к видео
        string pathVideo = await _videoRepository.GetPathVideoAsync(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        var directory = Path.GetDirectoryName(pathVideo)
                        ?? throw new DirectoryNotFoundException("Could not determine the directory of the video.");
        // Создаем временный файл для обрезанного видео
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        var setptsValue = (1 / optionsFfmpeg.SlowdownFactor).ToString(CultureInfo.InvariantCulture);
        var atempoValue = optionsFfmpeg.SlowdownFactor.ToString(CultureInfo.InvariantCulture);

        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{pathVideo}\"") // Входной файл
            .AddParameter($"-vf \"setpts={setptsValue}*PTS\"") // Видеофильтр (замедление)
            .AddParameter($"-af \"atempo={atempoValue}\"")     // Аудиофильтр (замедление)
            .SetOutput(tempFilePath)                          // Временный файл
            .SetOverwriteOutput(true);                        // Разрешаем перезапись

        // Запускаем конвертацию
        await conversion.Start();

        // Удаляем оригинальный файл
        if (File.Exists(pathVideo))
        {
            File.Delete(pathVideo);
        }

        // Переименовываем временный файл в оригинальное имя
        File.Move(tempFilePath, pathVideo);

        return pathVideo;
    }
    public async Task<string> SlowingVideoAsync(OptionsFfmpeg optionsFfmpeg)
    {
        // Получаем путь к видео
        string pathVideo = await _videoRepository.GetPathVideoAsync(optionsFfmpeg.VideoName, optionsFfmpeg.UserIdClaim);

        var directory = Path.GetDirectoryName(pathVideo)
                        ?? throw new DirectoryNotFoundException("Could not determine the directory of the video.");
        // Создаем временный файл для обрезанного видео
        var tempFilePath = Path.Combine(directory, "temp_" + Path.GetFileName(pathVideo));

        var slowdownFactor = optionsFfmpeg.SlowdownFactor;

        // Значение для фильтра setpts
        var setptsValue = (1 / slowdownFactor).ToString(CultureInfo.InvariantCulture);

        // Генерация фильтра atempo
        var atempoFilter = GenerateAtempoFilter(slowdownFactor);

        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{pathVideo}\"") // Входной файл
            .AddParameter($"-vf \"setpts={setptsValue}*PTS\"") // Видеофильтр (замедление)
            .AddParameter($"-af \"{atempoFilter}\"")          // Аудиофильтр (замедление)
            .SetOutput(tempFilePath)                          // Временный файл
            .SetOverwriteOutput(true);                        // Разрешаем перезапись                     // Разрешаем перезапись

        // Запускаем конвертацию
        await conversion.Start();

        // Удаляем оригинальный файл
        if (File.Exists(pathVideo))
        {
            File.Delete(pathVideo);
        }

        // Переименовываем временный файл в оригинальное имя
        File.Move(tempFilePath, pathVideo);

        return pathVideo;
    }
    private string GenerateAtempoFilter(double slowdownFactor)
    {
        var filters = new List<string>();

        // Умножаем или делим, пока значение не окажется в диапазоне от 0.5 до 2.0
        while (slowdownFactor < 0.5 || slowdownFactor > 2.0)
        {
            if (slowdownFactor < 0.5)
            {
                filters.Add("atempo=0.5");
                slowdownFactor *= 2; // Увеличиваем в 2 раза
            }
            else if (slowdownFactor > 2.0)
            {
                filters.Add("atempo=2.0");
                slowdownFactor /= 2; // Уменьшаем в 2 раза
            }
        }

        // Добавляем финальный фильтр с допустимым значением
        filters.Add($"atempo={slowdownFactor.ToString(CultureInfo.InvariantCulture)}");

        // Объединяем фильтры
        return string.Join(",", filters);
    }


    public async Task<FileStream> GetVideoFileAsync(string nameVideo, int id)
    {
        var videos = await _videoRepository.GetVideoByNameAsync(nameVideo, id); // метод должен возвращать один объект Video

        if (videos == null || string.IsNullOrEmpty(videos.PathVideo))
        {
            throw new FileNotFoundException("Video file not found.");
        }

        return new FileStream(videos.PathVideo, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    //public async Task GetAllVideosAsync()
    //{
    //    var video = await _videoRepository.GetAllAsync();
    //    return video;
    //}
    private const string PathVideo = "G:/video/";
    public async Task<string> SaveVideoAsync(IFormFile video, int userid)
    {
        var user = await _userRepository.GetUserByIdAsync(userid);

        if (video == null || video.Length <= 0)
        {
            throw new ArgumentException("Invalid video file.");
        }

        string pathVideo = PathVideo + user.Username;

        if (!Directory.Exists(pathVideo))
        {
            Directory.CreateDirectory(pathVideo);
        }

        var filePath = Path.Combine(pathVideo, video.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        var dbvideo = new Video(video.FileName, filePath, userid);

        await _videoRepository.AddPathVideo(dbvideo);

        return filePath;
    }

}

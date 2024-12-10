using API_website.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using API_website.Core.Models;
using Xabe.FFmpeg;
using System.IO;
using System.Net;
using System.Globalization;
using System.Drawing;
using Xabe.FFmpeg.Exceptions;
public class VideoService
{
    private const string PathVideo = "G:/video/";
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

    public async Task<string> SaveVideoAsync(IFormFile video, string nameVideo, string smile, int userid)
    {
        if (video == null || video.Length <= 0)
        {
            throw new ArgumentException("Invalid video file.");
        }

        nameVideo = $"{nameVideo}.mp4";

        Video existingVideo = await _videoRepository.GetVideoByNameAsync(nameVideo, userid);

        if (existingVideo != null)
        {
            throw new ArgumentException("A video with the same name already exists.");
        }

        var user = await _userRepository.GetUserByIdAsync(userid);

        string pathVideo = Path.Combine(PathVideo, user.Username);

        if (!Directory.Exists(pathVideo))
        {
            Directory.CreateDirectory(pathVideo);
        }

        var filePath = Path.Combine(pathVideo, nameVideo);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        var dbvideo = new Video(nameVideo, smile, filePath, userid);
        await _videoRepository.AddVideo(dbvideo);

        return filePath;
    }

    public async Task AddClipAsync(string nameVideo, IFormFile video, int idUser, bool point)
    {
        // Получить путь к первому видео
        string pathVideo = await _videoRepository.GetPathVideoAsync(nameVideo, idUser);

        // Путь для временного сохранения загруженного видео
        string uploadedVideoPath = Path.Combine(Path.GetTempPath(), video.FileName);

        // Сохранить видео на диск
        await using (var stream = new FileStream(uploadedVideoPath, FileMode.Create))
        {
            await video.CopyToAsync(stream);
        }

        // Проверить существование первого видео
        if (!File.Exists(pathVideo))
            throw new FileNotFoundException($"Первое видео не найдено: {pathVideo}");

        // Если файл в формате WebM, конвертируем его в MP4
        string secondVideoPath = uploadedVideoPath;
        if (Path.GetExtension(uploadedVideoPath).Equals(".webm", StringComparison.OrdinalIgnoreCase))
        {
            secondVideoPath = await ConvertWebMToMp4Async(uploadedVideoPath);

            // Удалить оригинальный WebM файл после конвертации
            if (File.Exists(uploadedVideoPath))
                File.Delete(uploadedVideoPath);
        }

        // Получение разрешений и размеров первого видео
        var mediaInfoFirst = await FFmpeg.GetMediaInfo(pathVideo);
        var firstVideoStream = mediaInfoFirst.VideoStreams.FirstOrDefault()
            ?? throw new InvalidOperationException("Первое видео не содержит видеопотока.");

        // Получение разрешений и размеров второго видео
        var mediaInfoSecond = await FFmpeg.GetMediaInfo(secondVideoPath);
        var secondVideoStream = mediaInfoSecond.VideoStreams.FirstOrDefault()
            ?? throw new InvalidOperationException("Второе видео не содержит видеопотока.");

        // Определяем, какое видео шире
        int width, height;
        if (firstVideoStream.Width > secondVideoStream.Width)
        {
            width = (firstVideoStream.Width % 2 == 0) ? firstVideoStream.Width : firstVideoStream.Width + 1;
            height = (firstVideoStream.Height % 2 == 0) ? firstVideoStream.Height : firstVideoStream.Height + 1;
        }
        else
        {
            width = (secondVideoStream.Width % 2 == 0) ? secondVideoStream.Width : secondVideoStream.Width + 1;
            height = (secondVideoStream.Height % 2 == 0) ? secondVideoStream.Height : secondVideoStream.Height + 1;
        }

        // Путь для масштабированного второго видео
        string scaledSecondVideoPath = Path.Combine(Path.GetTempPath(), $"scaled_{video.FileName}");

        await FFmpeg.Conversions.New()
            .AddParameter($"-i \"{secondVideoPath}\" -vf \"scale={width}:{height}:force_original_aspect_ratio=decrease,pad={width}:{height}:(ow-iw)/2:(oh-ih)/2:black\"")
            .SetOutput(scaledSecondVideoPath)
            .Start();

        string firstSecondVideoPath = Path.Combine(Path.GetTempPath(), $"first_{video.FileName}");

        // Масштабировать второе видео
        await FFmpeg.Conversions.New()
            .AddParameter($"-i \"{pathVideo}\" -vf \"scale={width}:{height}:force_original_aspect_ratio=decrease,pad={width}:{height}:(ow-iw)/2:(oh-ih)/2:black\"")
            .SetOutput(firstSecondVideoPath)
            .Start();

        // Путь для сохранения результата
        string outputVideoPath = Path.Combine(Path.GetTempPath(), $"output_{nameVideo}");

        if (point)
        {
            await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{firstSecondVideoPath}\" -i \"{scaledSecondVideoPath}\" -filter_complex \"[0:v:0][0:a:0][1:v:0][1:a:0]concat=n=2:v=1:a=1[outv][outa]\" -map \"[outv]\" -map \"[outa]\"")
                .SetOutput(outputVideoPath)
                .Start();
        }
        else
        {
            await FFmpeg.Conversions.New()
                .AddParameter($"-i \"{scaledSecondVideoPath}\" -i \"{firstSecondVideoPath}\" -filter_complex \"[0:v:0][0:a:0][1:v:0][1:a:0]concat=n=2:v=1:a=1[outv][outa]\" -map \"[outv]\" -map \"[outa]\"")
                .SetOutput(outputVideoPath)
                .Start();
        }
        // Объединить два видео


        // Заменить оригинальное видео новым
        if (File.Exists(pathVideo))
            File.Delete(pathVideo);

        File.Move(outputVideoPath, pathVideo);

        // Удалить временные файлы
        if (File.Exists(secondVideoPath))
            File.Delete(secondVideoPath);

        if (File.Exists(scaledSecondVideoPath))
            File.Delete(scaledSecondVideoPath);

        if (File.Exists(firstSecondVideoPath))
            File.Delete(firstSecondVideoPath);
    }

    public async Task<string> ConvertWebMToMp4Async(string inputFilePath)
    {
        if (!File.Exists(inputFilePath))
            throw new FileNotFoundException($"Файл {inputFilePath} не найден.");

        // Убедитесь, что расширение входного файла - .webm
        if (!Path.GetExtension(inputFilePath).Equals(".webm", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Файл должен быть в формате WebM.");

        // Путь для выходного файла
        string outputFilePath = Path.Combine(Path.GetTempPath(), $"{Path.GetFileNameWithoutExtension(inputFilePath)}.mp4");

        // Выполняем конвертацию WebM в MP4
        await FFmpeg.Conversions.New()
            .AddParameter($"-i \"{inputFilePath}\" -c:v libx264 -preset fast -crf 23 -c:a aac -b:a 192k")
            .SetOutput(outputFilePath)
            .Start();

        return outputFilePath;
    }


    public async Task<List<VideoInfo>> GetAllVideo(int idUser)
    {
        List<Video> videos = await _videoRepository.GetNameVideo(idUser);

        List<VideoInfo> listInfo = new();

        foreach (Video video in videos)
        {
            var mediaInfo = await FFmpeg.GetMediaInfo(video.PathVideo);

            VideoInfo videoInfo = new VideoInfo(video.NameVideo , mediaInfo.Duration.ToString(), video.Smile ?? "");

            listInfo.Add(videoInfo);
        }

        return listInfo;
    }
    public async Task DeleteVideoAsync(string videoName, int idUser)
    {
        try
        {
            string pathVideo = await _videoRepository.GetPathVideoAsync(videoName, idUser);

            if (File.Exists(pathVideo))
            {
                File.Delete(pathVideo);
            }

            await _videoRepository.DeleteVideoAsync(videoName, idUser);
        }
        catch (Exception ex)
        {
            throw new Exception("Not delete Exception: " + ex.Message, ex);
        }
    }

}

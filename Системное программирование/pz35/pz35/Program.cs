using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace DirectSoundApp;

class pz35
{
    static WaveInEvent? waveIn;
    static string recordedFile = "mic_record.wav";

    static void Main()
    {
        Console.WriteLine("=== ПРАКТИЧЕСКОЕ ЗАНЯТИЕ 35: DIRECTSOUND ===\n");

        Task1_CreateSoundBufferAndRecord();
        Task2_PlaySoundFromBuffer();
        Task3_GetCurrentPosition();
        Task4_MultichannelEffect();
        Task5_RecordAndSaveToFile();
        Task6_PlayFileWithDirectSound();
        Task7_VolumeControlWithKeyboard();
        Task8_FiltersAndEffects();
        Task9_VirtualConcertHall();
        Task10_KaraokeApp();

        Console.WriteLine("\nВсе задания выполнены!");
    }

    static void Task1_CreateSoundBufferAndRecord()
    {
        Console.WriteLine("\n1. Создание звукового буфера и запись с микрофона...");

        try
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(44100, 1);
            waveIn.DeviceNumber = 0;

            using (var writer = new WaveFileWriter(recordedFile, waveIn.WaveFormat))
            {
                waveIn.DataAvailable += (s, e) => writer.Write(e.Buffer, 0, e.BytesRecorded);

                Console.WriteLine("   Запись 5 секунд...");
                waveIn.StartRecording();
                Thread.Sleep(5000);
                waveIn.StopRecording();
            }

            Console.WriteLine($"   Звуковой буфер создан, данные сохранены в {recordedFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
        finally
        {
            waveIn?.Dispose();
        }
    }

    static void Task2_PlaySoundFromBuffer()
    {
        Console.WriteLine("\n2. Воспроизведение звука из буфера (DirectSoundOut)...");

        try
        {
            using (var reader = new AudioFileReader(recordedFile))
            using (var directSoundOut = new NAudio.Wave.DirectSoundOut())
            {
                directSoundOut.Init(reader);
                directSoundOut.Play();

                Console.WriteLine("   Воспроизведение 3 секунд...");
                Thread.Sleep(3000);
                directSoundOut.Stop();
            }

            Console.WriteLine("   Воспроизведение завершено");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task3_GetCurrentPosition()
    {
        Console.WriteLine("\n3. Определение текущей позиции в звуковом буфере...");

        try
        {
            using (var reader = new AudioFileReader(recordedFile))
            using (var waveOut = new NAudio.Wave.DirectSoundOut())
            {
                waveOut.Init(reader);
                waveOut.Play();

                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    long position = reader.Position;
                    long length = reader.Length;
                    double percent = (double)position / length * 100;
                    Console.WriteLine($"   Позиция: {position / 1000} кБ из {length / 1000} кБ ({percent:F1}%)");
                }

                waveOut.Stop();
            }

            Console.WriteLine("   Отслеживание позиции завершено");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task4_MultichannelEffect()
    {
        Console.WriteLine("\n4. Создание многоканального звукового эффекта...");

        string output = "multichannel_effect.wav";

        try
        {
            if (File.Exists(output))
            {
                File.Delete(output);
                Thread.Sleep(100);
            }

            var waveFormat = new NAudio.Wave.WaveFormat(44100, 6);

            using (var writer = new WaveFileWriter(output, waveFormat))
            {
                for (int i = 0; i < waveFormat.SampleRate * 3; i++)
                {
                    float t = (float)i / waveFormat.SampleRate;

                    float frontLeft = (float)(Math.Sin(2 * Math.PI * 440 * t) * 0.5);
                    float frontRight = (float)(Math.Sin(2 * Math.PI * 441 * t) * 0.5);
                    float center = (float)(Math.Sin(2 * Math.PI * 443 * t) * 0.3);
                    float subwoofer = (float)(Math.Sin(2 * Math.PI * 60 * t) * 0.2);
                    float rearLeft = (float)(Math.Sin(2 * Math.PI * 438 * t) * 0.4);
                    float rearRight = (float)(Math.Sin(2 * Math.PI * 442 * t) * 0.4);

                    byte[] buffer = new byte[24];
                    Buffer.BlockCopy(BitConverter.GetBytes(frontLeft), 0, buffer, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(frontRight), 0, buffer, 4, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(center), 0, buffer, 8, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(subwoofer), 0, buffer, 12, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(rearLeft), 0, buffer, 16, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(rearRight), 0, buffer, 20, 4);
                    writer.Write(buffer, 0, 24);
                }
            }

            Console.WriteLine($"   Многоканальный (5.1) эффект создан: {output}");

            using (var reader = new AudioFileReader(output))
            using (var dsOut = new NAudio.Wave.DirectSoundOut())
            {
                dsOut.Init(reader);
                dsOut.Play();
                Thread.Sleep(2000);
                dsOut.Stop();
            }

            Console.WriteLine("   Многоканальный эффект воспроизведен");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task5_RecordAndSaveToFile()
    {
        Console.WriteLine("\n5. Запись с аудиоустройства и сохранение в файл...");

        string output = "audio_device_record.wav";

        try
        {
            using (var waveInDevice = new WaveInEvent())
            using (var writer = new WaveFileWriter(output, new NAudio.Wave.WaveFormat(44100, 2)))
            {
                waveInDevice.WaveFormat = new NAudio.Wave.WaveFormat(44100, 2);
                waveInDevice.DataAvailable += (s, e) => writer.Write(e.Buffer, 0, e.BytesRecorded);

                Console.WriteLine("   Запись 5 секунд с аудиоустройства...");
                waveInDevice.StartRecording();
                Thread.Sleep(5000);
                waveInDevice.StopRecording();
            }

            Console.WriteLine($"   Сохранено в {output}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task6_PlayFileWithDirectSound()
    {
        Console.WriteLine("\n6. Воспроизведение файла через DirectSound...");

        string fileToPlay = "multichannel_effect.wav";
        if (!File.Exists(fileToPlay)) fileToPlay = recordedFile;

        try
        {
            using (var reader = new AudioFileReader(fileToPlay))
            using (var dsOut = new NAudio.Wave.DirectSoundOut())
            {
                dsOut.Init(reader);
                Console.WriteLine($"   Воспроизведение {fileToPlay}...");
                dsOut.Play();
                Thread.Sleep(4000);
                dsOut.Stop();
            }

            Console.WriteLine("   Воспроизведение завершено");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task7_VolumeControlWithKeyboard()
    {
        Console.WriteLine("\n7. Управление громкостью (клавиатура ВВЕРХ/ВНИЗ)...");
        Console.WriteLine("   Используйте стрелки ВВЕРХ/ВНИЗ для изменения громкости");
        Console.WriteLine("   Нажмите ENTER для выхода");

        try
        {
            using (var reader = new AudioFileReader(recordedFile))
            using (var dsOut = new NAudio.Wave.DirectSoundOut())
            {
                var volumeProvider = new VolumeSampleProvider(reader);
                dsOut.Init(volumeProvider);
                dsOut.Play();

                float currentVolume = 0.5f;
                bool running = true;

                while (running)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        switch (key.Key)
                        {
                            case ConsoleKey.UpArrow:
                                currentVolume = Math.Min(1.0f, currentVolume + 0.05f);
                                volumeProvider.Volume = currentVolume;
                                Console.WriteLine($"   Громкость: {currentVolume * 100:F0}%");
                                break;
                            case ConsoleKey.DownArrow:
                                currentVolume = Math.Max(0.0f, currentVolume - 0.05f);
                                volumeProvider.Volume = currentVolume;
                                Console.WriteLine($"   Громкость: {currentVolume * 100:F0}%");
                                break;
                            case ConsoleKey.Enter:
                                running = false;
                                break;
                        }
                    }
                    Thread.Sleep(50);
                }

                dsOut.Stop();
            }

            Console.WriteLine("   Управление громкостью завершено");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task8_FiltersAndEffects()
    {
        Console.WriteLine("\n8. Обработка звука с фильтрами и эффектами...");

        string output = "filtered_audio.wav";

        try
        {
            if (File.Exists(output))
            {
                File.Delete(output);
                Thread.Sleep(100);
            }

            using (var reader = new AudioFileReader(recordedFile))
            {
                float[] samples = new float[reader.Length / 4];
                reader.Read(samples, 0, samples.Length);

                float[] filtered = new float[samples.Length];
                float alpha = 0.3f;
                filtered[0] = samples[0];
                for (int i = 1; i < samples.Length; i++)
                {
                    filtered[i] = alpha * samples[i] + (1 - alpha) * filtered[i - 1];
                }

                using (var writer = new WaveFileWriter(output, reader.WaveFormat))
                {
                    writer.WriteSamples(filtered, 0, filtered.Length);
                }
            }

            Console.WriteLine($"   Low-pass фильтр применен, сохранен в {output}");

            using (var filteredReader = new AudioFileReader(output))
            using (var dsOut = new NAudio.Wave.DirectSoundOut())
            {
                dsOut.Init(filteredReader);
                dsOut.Play();
                Thread.Sleep(3000);
                dsOut.Stop();
            }

            Console.WriteLine("   Эффекты применены и воспроизведены");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task9_VirtualConcertHall()
    {
        Console.WriteLine("\n9. Виртуальный концертный зал с объемным звучанием...");

        string output = "concert_hall.wav";

        try
        {
            if (File.Exists(output))
            {
                File.Delete(output);
                Thread.Sleep(100);
            }

            var waveFormat = new NAudio.Wave.WaveFormat(44100, 2);

            using (var writer = new WaveFileWriter(output, waveFormat))
            {
                for (int i = 0; i < waveFormat.SampleRate * 6; i++)
                {
                    float t = (float)i / waveFormat.SampleRate;

                    float position = (float)Math.Sin(t * 0.5);
                    float leftVol = (position < 0) ? 1.0f : (1 - position);
                    float rightVol = (position > 0) ? 1.0f : (1 + position);
                    leftVol = Math.Clamp(leftVol, 0.2f, 1.0f);
                    rightVol = Math.Clamp(rightVol, 0.2f, 1.0f);

                    float sample = (float)(
                        Math.Sin(2 * Math.PI * 440 * t) * 0.4 +
                        Math.Sin(2 * Math.PI * 880 * t) * 0.2 +
                        Math.Sin(2 * Math.PI * 1320 * t) * 0.1
                    );

                    float reverb = 0;
                    if (t > 0.1) reverb = (float)(Math.Sin(2 * Math.PI * 440 * (t - 0.1)) * 0.15);

                    float left = sample * leftVol + reverb;
                    float right = sample * rightVol + reverb;

                    byte[] buffer = new byte[8];
                    Buffer.BlockCopy(BitConverter.GetBytes(left), 0, buffer, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(right), 0, buffer, 4, 4);
                    writer.Write(buffer, 0, 8);
                }
            }

            Console.WriteLine($"   Концертный зал создан: {output}");

            using (var reader = new AudioFileReader(output))
            using (var dsOut = new NAudio.Wave.DirectSoundOut())
            {
                dsOut.Init(reader);
                dsOut.Play();
                Thread.Sleep(4000);
                dsOut.Stop();
            }

            Console.WriteLine("   Объемное звучание воспроизведено");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }

    static void Task10_KaraokeApp()
    {
        Console.WriteLine("\n10. Караоке-приложение...");

        string output = "karaoke_backing.wav";

        try
        {
            if (File.Exists(output))
            {
                File.Delete(output);
                Thread.Sleep(100);
            }

            var waveFormat = new NAudio.Wave.WaveFormat(44100, 2);

            using (var writer = new WaveFileWriter(output, waveFormat))
            {
                for (int i = 0; i < waveFormat.SampleRate * 8; i++)
                {
                    float t = (float)i / waveFormat.SampleRate;

                    float bass = (float)(Math.Sin(2 * Math.PI * 110 * t) * 0.3);
                    float melody = (float)(
                        Math.Sin(2 * Math.PI * 440 * t) * 0.25 +
                        Math.Sin(2 * Math.PI * 554 * t) * 0.2 +
                        Math.Sin(2 * Math.PI * 659 * t) * 0.15
                    );
                    float drums = ((int)(t * 4) % 2 == 0) ? 0.1f : 0;

                    float mix = bass + melody + drums;

                    byte[] buffer = new byte[8];
                    Buffer.BlockCopy(BitConverter.GetBytes(mix), 0, buffer, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(mix), 0, buffer, 4, 4);
                    writer.Write(buffer, 0, 8);
                }
            }

            Console.WriteLine($"   Минусовка создана: {output}");
            Console.WriteLine("   Для караоке спойте поверх этой мелодии!");

            using (var reader = new AudioFileReader(output))
            using (var dsOut = new NAudio.Wave.DirectSoundOut())
            {
                dsOut.Init(reader);
                dsOut.Play();

                Console.WriteLine("   Минусовка играет... (5 секунд)");
                Console.WriteLine("   🎤 Пойте вместе с мелодией!");
                Thread.Sleep(5000);
                dsOut.Stop();
            }

            Console.WriteLine("   Караоке-сессия завершена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка: {ex.Message}");
        }
    }
}
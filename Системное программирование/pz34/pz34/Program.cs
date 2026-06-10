using System.Text;

namespace RIFFProcessingApp;

class Program
{
    static void Main()
    {
        Step1_CreateRIFFAudioFile();
        Step2_ReadRIFFAndPrint();
        Step3_WriteDataToRIFF();
        Step4_CreateMetadata();
        Step5_ReadMetadata();
        Step6_ModifyMetadata();
        Step7_CreateAudioContainer();
        Step8_InsertAudioData();
        Step9_ReadAndPlayAudio();
        Step10_CreateVideoContainer();
        Step11_InsertVideoData();
        Step12_ReadAndDisplayVideoInfo();
    }

    static void Step1_CreateRIFFAudioFile()
    {
        string filename = "audio_riff.wav";
        int sampleRate = 44100;
        int duration = 3;
        int numChannels = 1;
        int bitsPerSample = 16;

        using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));

        int byteRate = sampleRate * numChannels * bitsPerSample / 8;
        short blockAlign = (short)(numChannels * bitsPerSample / 8);
        int dataSize = sampleRate * duration * numChannels * bitsPerSample / 8;

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(36 + dataSize);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)numChannels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write(blockAlign);
        writer.Write((short)bitsPerSample);

        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(dataSize);

        for (int i = 0; i < sampleRate * duration; i++)
        {
            float t = (float)i / sampleRate;
            short sample = (short)(32760 * Math.Sin(2 * Math.PI * 440 * t));
            writer.Write(sample);
        }

        Console.WriteLine($"1. RIFF аудиофайл создан: {filename}");
    }

    static void Step2_ReadRIFFAndPrint()
    {
        string filename = "audio_riff.wav";
        if (!File.Exists(filename)) return;

        using var reader = new BinaryReader(File.Open(filename, FileMode.Open));

        string riffId = Encoding.ASCII.GetString(reader.ReadBytes(4));
        int fileSize = reader.ReadInt32();
        string waveId = Encoding.ASCII.GetString(reader.ReadBytes(4));

        Console.WriteLine($"2. RIFF файл: {filename}");
        Console.WriteLine($"   ID: {riffId}, Размер: {fileSize}, Формат: {waveId}");

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            int chunkSize = reader.ReadInt32();
            Console.WriteLine($"   Чанк: {chunkId}, Размер: {chunkSize}");

            if (chunkId == "fmt ")
            {
                short audioFormat = reader.ReadInt16();
                short numChannels = reader.ReadInt16();
                int sampleRate = reader.ReadInt32();
                int byteRate = reader.ReadInt32();
                short blockAlign = reader.ReadInt16();
                short bitsPerSample = reader.ReadInt16();
                Console.WriteLine($"     Аудио формат: {audioFormat}, Каналов: {numChannels}");
                Console.WriteLine($"     Частота: {sampleRate}, Битрейт: {byteRate}");
            }
            else if (chunkId == "data")
            {
                Console.WriteLine($"     Аудиоданные: {chunkSize} байт");
                reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
            }
            else
            {
                reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
            }
        }
    }

    static void Step3_WriteDataToRIFF()
    {
        string filename = "modified_riff.wav";

        using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(0);
        long sizePos = writer.BaseStream.Position;

        writer.Write(Encoding.ASCII.GetBytes("WAVE"));
        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)2);
        writer.Write(44100);
        writer.Write(44100 * 2 * 2);
        writer.Write((short)4);
        writer.Write((short)16);

        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(0);
        long dataPos = writer.BaseStream.Position;

        for (int i = 0; i < 44100 * 2; i++)
        {
            short left = (short)(20000 * Math.Sin(2 * Math.PI * 440 * i / 44100));
            short right = (short)(20000 * Math.Sin(2 * Math.PI * 441 * i / 44100));
            writer.Write(left);
            writer.Write(right);
        }

        long endPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(dataPos - 4, SeekOrigin.Begin);
        writer.Write((int)(endPos - dataPos));
        writer.BaseStream.Seek(sizePos, SeekOrigin.Begin);
        writer.Write((int)(endPos - 8));

        Console.WriteLine($"3. Данные записаны в RIFF: {filename}");
    }

    static void Step4_CreateMetadata()
    {
        string filename = "riff_with_meta.wav";

        using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(0);
        long sizePos = writer.BaseStream.Position;
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(44100);
        writer.Write(44100 * 2);
        writer.Write((short)2);
        writer.Write((short)16);

        writer.Write(Encoding.ASCII.GetBytes("LIST"));
        writer.Write(0);
        long listPos = writer.BaseStream.Position;
        writer.Write(Encoding.ASCII.GetBytes("INFO"));

        writer.Write(Encoding.ASCII.GetBytes("INAM"));
        writer.Write(12);
        writer.Write(Encoding.ASCII.GetBytes("Test Song\0"));

        writer.Write(Encoding.ASCII.GetBytes("IART"));
        writer.Write(10);
        writer.Write(Encoding.ASCII.GetBytes("Student\0"));

        long endListPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(listPos, SeekOrigin.Begin);
        writer.Write((int)(endListPos - listPos - 4));

        writer.BaseStream.Seek(endListPos, SeekOrigin.Begin);
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(44100 * 2);
        for (int i = 0; i < 44100 * 2; i++)
        {
            short sample = (short)(30000 * Math.Sin(2 * Math.PI * 440 * i / 44100));
            writer.Write(sample);
        }

        long endPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(sizePos, SeekOrigin.Begin);
        writer.Write((int)(endPos - 8));

        Console.WriteLine($"4. Метаданные созданы: {filename}");
    }

    static void Step5_ReadMetadata()
    {
        string filename = "riff_with_meta.wav";
        if (!File.Exists(filename)) return;

        using var reader = new BinaryReader(File.Open(filename, FileMode.Open));

        reader.ReadBytes(12);

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            int chunkSize = reader.ReadInt32();

            if (chunkId == "LIST")
            {
                string listType = Encoding.ASCII.GetString(reader.ReadBytes(4));
                if (listType == "INFO")
                {
                    int infoEnd = (int)reader.BaseStream.Position + chunkSize - 4;
                    while (reader.BaseStream.Position < infoEnd)
                    {
                        string infoId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                        int infoSize = reader.ReadInt32();
                        string infoValue = Encoding.ASCII.GetString(reader.ReadBytes(infoSize));
                        Console.WriteLine($"   Метаданные: {infoId} = {infoValue.TrimEnd('\0')}");
                    }
                }
                else
                {
                    reader.BaseStream.Seek(chunkSize - 4, SeekOrigin.Current);
                }
            }
            else
            {
                reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
            }
        }

        Console.WriteLine("5. Метаданные прочитаны");
    }

    static void Step6_ModifyMetadata()
    {
        string filename = "riff_with_meta.wav";
        if (!File.Exists(filename)) return;

        byte[] fileData = File.ReadAllBytes(filename);
        using var ms = new MemoryStream(fileData);
        using var reader = new BinaryReader(ms);
        using var writer = new BinaryWriter(File.Open("riff_modified_meta.wav", FileMode.Create));

        writer.Write(reader.ReadBytes(12));

        while (ms.Position < ms.Length)
        {
            string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            int chunkSize = reader.ReadInt32();
            writer.Write(Encoding.ASCII.GetBytes(chunkId));
            writer.Write(chunkSize);

            if (chunkId == "LIST")
            {
                string listType = Encoding.ASCII.GetString(reader.ReadBytes(4));
                writer.Write(Encoding.ASCII.GetBytes(listType));

                int infoEnd = (int)ms.Position + chunkSize - 4;
                while (ms.Position < infoEnd)
                {
                    string infoId = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    int infoSize = reader.ReadInt32();
                    string infoValue = Encoding.ASCII.GetString(reader.ReadBytes(infoSize));

                    if (infoId == "INAM")
                    {
                        infoValue = "Modified Song Title";
                        infoSize = infoValue.Length + 1;
                        writer.Write(Encoding.ASCII.GetBytes(infoId));
                        writer.Write(infoSize);
                        writer.Write(Encoding.ASCII.GetBytes(infoValue + "\0"));
                    }
                    else
                    {
                        writer.Write(Encoding.ASCII.GetBytes(infoId));
                        writer.Write(infoSize);
                        writer.Write(Encoding.ASCII.GetBytes(infoValue));
                    }
                }
            }
            else
            {
                byte[] data = reader.ReadBytes(chunkSize);
                writer.Write(data);
            }
        }

        Console.WriteLine("6. Метаданные изменены: riff_modified_meta.wav");
    }

    static void Step7_CreateAudioContainer()
    {
        string filename = "audio_container.riff";

        using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(0);
        long sizePos = writer.BaseStream.Position;
        writer.Write(Encoding.ASCII.GetBytes("AUDI"));

        writer.Write(Encoding.ASCII.GetBytes("strh"));
        writer.Write(20);
        writer.Write(Encoding.ASCII.GetBytes("auds"));
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(44100);

        writer.Write(Encoding.ASCII.GetBytes("strf"));
        writer.Write(18);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(44100);
        writer.Write(88200);
        writer.Write((short)2);
        writer.Write((short)16);

        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(0);
        long dataPos = writer.BaseStream.Position;

        for (int i = 0; i < 88200; i++)
        {
            short sample = (short)(20000 * Math.Sin(2 * Math.PI * 440 * i / 44100));
            writer.Write(sample);
        }

        long endPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(dataPos - 4, SeekOrigin.Begin);
        writer.Write((int)(endPos - dataPos));
        writer.BaseStream.Seek(sizePos, SeekOrigin.Begin);
        writer.Write((int)(endPos - 8));

        Console.WriteLine($"7. Аудиоконтейнер создан: {filename}");
    }

    static void Step8_InsertAudioData()
    {
        string filename = "audio_container.riff";
        if (!File.Exists(filename)) return;

        byte[] fileData = File.ReadAllBytes(filename);
        var ms = new MemoryStream();

        using var writer = new BinaryWriter(ms);
        writer.Write(fileData);

        int dataChunkPos = FindChunkPosition(fileData, "data");
        if (dataChunkPos >= 0)
        {
            ms.Seek(dataChunkPos, SeekOrigin.Begin);
            ms.Seek(8, SeekOrigin.Current);

            for (int i = 0; i < 44100 * 2; i++)
            {
                short sample = (short)(25000 * Math.Sin(2 * Math.PI * 523 * i / 44100));
                writer.Write(sample);
            }
        }

        File.WriteAllBytes("audio_container_filled.riff", ms.ToArray());
        Console.WriteLine("8. Аудиоданные вставлены в контейнер");
    }

    static void Step9_ReadAndPlayAudio()
    {
        string filename = "audio_container_filled.riff";
        if (!File.Exists(filename))
        {
            filename = "audio_riff.wav";
        }

        using var reader = new BinaryReader(File.Open(filename, FileMode.Open));

        reader.ReadBytes(12);

        int dataSize = 0;
        byte[]? audioData = null;

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            int chunkSize = reader.ReadInt32();

            if (chunkId == "data")
            {
                dataSize = chunkSize;
                audioData = reader.ReadBytes(chunkSize);
                break;
            }
            else
            {
                reader.ReadBytes(chunkSize);
            }
        }

        if (audioData != null)
        {
            Console.WriteLine($"9. Аудиоданные прочитаны: {dataSize} байт");
            Console.WriteLine("   Для воспроизведения используйте любой аудиоплеер");
        }
    }

    static void Step10_CreateVideoContainer()
    {
        string filename = "video_container.avi";

        using var writer = new BinaryWriter(File.Open(filename, FileMode.Create));

        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(0);
        long sizePos = writer.BaseStream.Position;
        writer.Write(Encoding.ASCII.GetBytes("AVI "));

        writer.Write(Encoding.ASCII.GetBytes("LIST"));
        writer.Write(0);
        long listPos = writer.BaseStream.Position;
        writer.Write(Encoding.ASCII.GetBytes("hdrl"));

        writer.Write(Encoding.ASCII.GetBytes("avih"));
        writer.Write(56);
        writer.Write(1000000);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(100);
        writer.Write(30);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);

        writer.Write(Encoding.ASCII.GetBytes("LIST"));
        writer.Write(0);
        long strlPos = writer.BaseStream.Position;
        writer.Write(Encoding.ASCII.GetBytes("strl"));

        writer.Write(Encoding.ASCII.GetBytes("strh"));
        writer.Write(56);
        writer.Write(Encoding.ASCII.GetBytes("vids"));
        writer.Write(Encoding.ASCII.GetBytes("dib "));
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(30);

        writer.Write(Encoding.ASCII.GetBytes("strf"));
        writer.Write(40);
        writer.Write(640);
        writer.Write(480);
        writer.Write(0);
        writer.Write(0);
        writer.Write(24);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);
        writer.Write(0);

        long endStrlPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(strlPos, SeekOrigin.Begin);
        writer.Write((int)(endStrlPos - strlPos - 4));

        writer.BaseStream.Seek(endStrlPos, SeekOrigin.Begin);
        long endListPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(listPos, SeekOrigin.Begin);
        writer.Write((int)(endListPos - listPos - 4));

        writer.BaseStream.Seek(endListPos, SeekOrigin.Begin);
        writer.Write(Encoding.ASCII.GetBytes("LIST"));
        writer.Write(0);
        long moviPos = writer.BaseStream.Position;
        writer.Write(Encoding.ASCII.GetBytes("movi"));

        writer.Write(Encoding.ASCII.GetBytes("00db"));
        writer.Write(640 * 480 * 3);

        for (int y = 0; y < 480; y++)
        {
            for (int x = 0; x < 640; x++)
            {
                byte r = (byte)((x + y) % 256);
                byte g = (byte)((x * 2 + y) % 256);
                byte b = (byte)((y * 2) % 256);
                writer.Write(b);
                writer.Write(g);
                writer.Write(r);
            }
        }

        long endMoviPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(moviPos, SeekOrigin.Begin);
        writer.Write((int)(endMoviPos - moviPos - 4));

        long endPos = writer.BaseStream.Position;
        writer.BaseStream.Seek(sizePos, SeekOrigin.Begin);
        writer.Write((int)(endPos - 8));

        Console.WriteLine($"10. Видеоконтейнер создан: {filename}");
    }

    static void Step11_InsertVideoData()
    {
        string filename = "video_container.avi";
        if (!File.Exists(filename)) return;

        byte[] fileData = File.ReadAllBytes(filename);
        var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(fileData);

        int moviPos = FindChunkPosition(fileData, "movi");
        if (moviPos >= 0)
        {
            ms.Seek(moviPos + 8, SeekOrigin.Begin);
            writer.Write(Encoding.ASCII.GetBytes("00db"));
            writer.Write(640 * 480 * 3);

            for (int y = 0; y < 480; y++)
            {
                for (int x = 0; x < 640; x++)
                {
                    byte r = (byte)((x + y) % 256);
                    byte g = (byte)((x * 2 + y) % 256);
                    byte b = (byte)((y * 2) % 256);
                    writer.Write(b);
                    writer.Write(g);
                    writer.Write(r);
                }
            }
        }

        File.WriteAllBytes("video_container_filled.avi", ms.ToArray());
        Console.WriteLine("11. Видеоданные вставлены в контейнер");
    }

    static void Step12_ReadAndDisplayVideoInfo()
    {
        string filename = "video_container_filled.avi";
        if (!File.Exists(filename)) return;

        using var reader = new BinaryReader(File.Open(filename, FileMode.Open));

        string riffId = Encoding.ASCII.GetString(reader.ReadBytes(4));
        int fileSize = reader.ReadInt32();
        string aviId = Encoding.ASCII.GetString(reader.ReadBytes(4));

        Console.WriteLine($"12. Видеоконтейнер: {filename}");
        Console.WriteLine($"    ID: {riffId}, Размер: {fileSize}, Формат: {aviId}");

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            string chunkId = Encoding.ASCII.GetString(reader.ReadBytes(4));
            int chunkSize = reader.ReadInt32();

            if (chunkId == "avih")
            {
                int microsecPerFrame = reader.ReadInt32();
                int maxBytesPerSec = reader.ReadInt32();
                int dwFlags = reader.ReadInt32();
                Console.WriteLine($"    Кадров в секунду: {1000000 / microsecPerFrame}");
                reader.ReadBytes(chunkSize - 12);
            }
            else if (chunkId == "strh")
            {
                string type = Encoding.ASCII.GetString(reader.ReadBytes(4));
                reader.ReadBytes(16);
                int frames = reader.ReadInt32();
                if (type == "vids")
                    Console.WriteLine($"    Видеопоток: {frames} кадров");
                reader.ReadBytes(chunkSize - 24);
            }
            else
            {
                reader.ReadBytes(chunkSize);
            }
        }
    }

    static int FindChunkPosition(byte[] data, string chunkId)
    {
        byte[] idBytes = Encoding.ASCII.GetBytes(chunkId);
        for (int i = 0; i < data.Length - 4; i++)
        {
            if (data[i] == idBytes[0] && data[i + 1] == idBytes[1] &&
                data[i + 2] == idBytes[2] && data[i + 3] == idBytes[3])
            {
                return i;
            }
        }
        return -1;
    }
}
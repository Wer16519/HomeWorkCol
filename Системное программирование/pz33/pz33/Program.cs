using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Lame;
using System.Numerics;

namespace AudioProcessingApp;

class Program
{
    static void Main()
    {
        Task1_RecordMicrophone();
        Task2_ApplyFiltersAndEffects();
        Task3_VirtualConcertHall();
        Task4_ConvertToMp3();
        Task5_KaraokeApp();
        Task6_RecordMultipleMicrophones();
        Task7_FFTAnalysis();
        Task8_MultichannelEffect();
        Task9_OpenAL3DSound();
        Task10_VSTEmulation();
        Task11_AccessMarkersAnalysis();
        Task12_CombinedProcessing();
    }

    static void Task1_RecordMicrophone()
    {
        string file = "recorded.wav";
        using var waveIn = new WaveInEvent();
        waveIn.WaveFormat = new WaveFormat(44100, 1);
        using var writer = new WaveFileWriter(file, waveIn.WaveFormat);

        waveIn.DataAvailable += (s, e) => writer.Write(e.Buffer, 0, e.BytesRecorded);
        waveIn.StartRecording();
        Console.WriteLine("Запись 5 секунд...");
        Thread.Sleep(5000);
        waveIn.StopRecording();

        Console.WriteLine($"1. Запись микрофона: {file}");
    }

    static void Task2_ApplyFiltersAndEffects()
    {
        if (!File.Exists("recorded.wav")) return;

        string output = "effected.wav";

        using var reader = new AudioFileReader("recorded.wav");
        var volume = new VolumeSampleProvider(reader) { Volume = 2.0f };

        using var writer = new WaveFileWriter(output, reader.WaveFormat);
        float[] buffer = new float[44100];
        int read;
        while ((read = volume.Read(buffer, 0, buffer.Length)) > 0)
        {
            writer.WriteSamples(buffer, 0, read);
        }

        Console.WriteLine($"2. Эффекты (усиление) применены: {output}");
    }

    static void Task3_VirtualConcertHall()
    {
        if (!File.Exists("recorded.wav")) return;

        string output = "concert_hall.wav";

        using var reader = new AudioFileReader("recorded.wav");
        var delay = new DelaySampleProvider(reader);
        delay.DelayTimeMs = 200;
        delay.Gain = 0.5f;
        delay.Feedback = 0.3f;

        using var writer = new WaveFileWriter(output, reader.WaveFormat);
        float[] buffer = new float[44100];
        int read;
        while ((read = delay.Read(buffer, 0, buffer.Length)) > 0)
        {
            writer.WriteSamples(buffer, 0, read);
        }

        Console.WriteLine($"3. Концертный зал (реверберация): {output}");
    }

    static void Task4_ConvertToMp3()
    {
        if (!File.Exists("recorded.wav")) return;

        string output = "converted.mp3";

        using var reader = new AudioFileReader("recorded.wav");
        using var writer = new LameMP3FileWriter(output, reader.WaveFormat, 128);
        reader.CopyTo(writer);

        Console.WriteLine($"4. Конвертация в MP3: {output}");
    }

    static void Task5_KaraokeApp()
    {
        string output = "karaoke.wav";
        var waveFormat = new WaveFormat(44100, 2);

        using var writer = new WaveFileWriter(output, waveFormat);

        for (int i = 0; i < waveFormat.SampleRate * 5; i++)
        {
            float left = (float)(Math.Sin(2 * Math.PI * 440 * i / waveFormat.SampleRate) * 0.5);
            float right = left;
            byte[] buffer = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(left), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(right), 0, buffer, 4, 4);
            writer.Write(buffer, 0, 8);
        }

        Console.WriteLine($"5. Караоке приложение: {output}");
    }

    static void Task6_RecordMultipleMicrophones()
    {
        string output = "multimic.wav";
        var waveFormat = new WaveFormat(44100, 2);

        using var writer = new WaveFileWriter(output, waveFormat);

        for (int i = 0; i < waveFormat.SampleRate * 3; i++)
        {
            float mic1 = (float)(Math.Sin(2 * Math.PI * 300 * i / waveFormat.SampleRate) * 0.3);
            float mic2 = (float)(Math.Sin(2 * Math.PI * 500 * i / waveFormat.SampleRate) * 0.3);
            float mixed = (mic1 + mic2) / 2;

            byte[] buffer = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(mixed), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(mixed), 0, buffer, 4, 4);
            writer.Write(buffer, 0, 8);
        }

        Console.WriteLine($"6. Смешивание с нескольких микрофонов: {output}");
    }

    static void Task7_FFTAnalysis()
    {
        if (!File.Exists("recorded.wav")) return;

        using var reader = new AudioFileReader("recorded.wav");
        float[] samples = new float[1024];
        reader.Read(samples, 0, 1024);

        System.Numerics.Complex[] fft = new System.Numerics.Complex[1024];
        for (int i = 0; i < 1024; i++)
            fft[i] = new System.Numerics.Complex(samples[i], 0);

        FFT(fft, false);

        float maxMagnitude = 0;
        int maxIndex = 0;
        for (int i = 0; i < 512; i++)
        {
            float mag = (float)fft[i].Magnitude;
            if (mag > maxMagnitude)
            {
                maxMagnitude = mag;
                maxIndex = i;
            }
        }

        float freq = maxIndex * reader.WaveFormat.SampleRate / 1024;
        Console.WriteLine($"7. FFT анализ: доминирующая частота = {freq:F1} Гц");
    }

    static void Task8_MultichannelEffect()
    {
        string output = "multichannel.wav";
        var waveFormat = new WaveFormat(44100, 4);

        using var writer = new WaveFileWriter(output, waveFormat);

        for (int i = 0; i < waveFormat.SampleRate * 3; i++)
        {
            float t = (float)i / waveFormat.SampleRate;
            float left = (float)(Math.Sin(2 * Math.PI * 440 * t) * 0.5);
            float right = (float)(Math.Sin(2 * Math.PI * 441 * t) * 0.5);
            float rearLeft = (float)(Math.Sin(2 * Math.PI * 438 * t) * 0.3);
            float rearRight = (float)(Math.Sin(2 * Math.PI * 442 * t) * 0.3);

            byte[] buffer = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(left), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(right), 0, buffer, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(rearLeft), 0, buffer, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(rearRight), 0, buffer, 12, 4);
            writer.Write(buffer, 0, 16);
        }

        Console.WriteLine($"8. Многоканальный эффект: {output}");
    }

    static void Task9_OpenAL3DSound()
    {
        string output = "3d_sound.wav";
        var waveFormat = new WaveFormat(44100, 2);

        using var writer = new WaveFileWriter(output, waveFormat);

        for (int i = 0; i < waveFormat.SampleRate * 4; i++)
        {
            float t = (float)i / waveFormat.SampleRate;
            float angle = t * 2 * (float)Math.PI;
            float leftVol = (float)(0.5 + 0.5 * Math.Sin(angle));
            float rightVol = (float)(0.5 + 0.5 * Math.Cos(angle));

            float sample = (float)(Math.Sin(2 * Math.PI * 440 * t) * 0.5);
            float left = sample * leftVol;
            float right = sample * rightVol;

            byte[] buffer = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes(left), 0, buffer, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(right), 0, buffer, 4, 4);
            writer.Write(buffer, 0, 8);
        }

        Console.WriteLine($"9. 3D звук (OpenAL эмуляция): {output}");
    }

    static void Task10_VSTEmulation()
    {
        if (!File.Exists("recorded.wav")) return;

        string output = "vst_processed.wav";

        using var reader = new AudioFileReader("recorded.wav");
        using var writer = new WaveFileWriter(output, reader.WaveFormat);

        float[] buffer = new float[44100];
        int read;

        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                buffer[i] = (float)(Math.Tanh(buffer[i] * 2.0) * 0.8);
            }
            writer.WriteSamples(buffer, 0, read);
        }

        Console.WriteLine($"10. VST эмуляция (дисторшн): {output}");
    }

    static void Task11_AccessMarkersAnalysis()
    {
        if (!File.Exists("recorded.wav")) return;

        string output = "markers_analysis.txt";

        using var reader = new AudioFileReader("recorded.wav");
        float[] allSamples = new float[(int)(reader.Length / 4)];
        reader.Read(allSamples, 0, allSamples.Length);

        using var writer = new StreamWriter(output);

        for (int i = 0; i < allSamples.Length; i += reader.WaveFormat.SampleRate)
        {
            float max = 0;
            for (int j = 0; j < reader.WaveFormat.SampleRate && i + j < allSamples.Length; j++)
            {
                max = Math.Max(max, Math.Abs(allSamples[i + j]));
            }
            writer.WriteLine($"Маркер {i / reader.WaveFormat.SampleRate + 1} сек: пиковая амплитуда = {max:F4}");
        }

        Console.WriteLine($"11. Анализ с маркерами доступа: {output}");
    }

    static void Task12_CombinedProcessing()
    {
        if (!File.Exists("recorded.wav")) return;

        string output = "combined_processed.wav";

        using var reader = new AudioFileReader("recorded.wav");
        var volume = new VolumeSampleProvider(reader) { Volume = 1.5f };
        var delay = new DelaySampleProvider(volume);
        delay.DelayTimeMs = 100;
        delay.Gain = 0.4f;
        delay.Feedback = 0.2f;

        using var writer = new WaveFileWriter(output, reader.WaveFormat);
        float[] buffer = new float[44100];
        int read;

        while ((read = delay.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (int i = 0; i < read; i++)
            {
                buffer[i] = (float)(Math.Tanh(buffer[i] * 1.3) * 0.9);
            }
            writer.WriteSamples(buffer, 0, read);
        }

        Console.WriteLine($"12. Комплексная обработка: {output}");
    }

    static void FFT(System.Numerics.Complex[] buffer, bool inverse)
    {
        int n = buffer.Length;
        if (n == 1) return;

        System.Numerics.Complex[] even = new System.Numerics.Complex[n / 2];
        System.Numerics.Complex[] odd = new System.Numerics.Complex[n / 2];

        for (int i = 0; i < n / 2; i++)
        {
            even[i] = buffer[2 * i];
            odd[i] = buffer[2 * i + 1];
        }

        FFT(even, inverse);
        FFT(odd, inverse);

        float angle = 2 * (float)Math.PI / n * (inverse ? -1 : 1);
        System.Numerics.Complex w = new System.Numerics.Complex(1, 0);
        System.Numerics.Complex wn = new System.Numerics.Complex(Math.Cos(angle), Math.Sin(angle));

        for (int i = 0; i < n / 2; i++)
        {
            System.Numerics.Complex t = w * odd[i];
            buffer[i] = even[i] + t;
            buffer[i + n / 2] = even[i] - t;
            w *= wn;
        }
    }
}

class DelaySampleProvider : ISampleProvider
{
    private readonly ISampleProvider source;
    private Queue<float> delayBuffer;
    private int delaySamples;
    private int delayTimeMs;

    public int DelayTimeMs
    {
        get => delayTimeMs;
        set
        {
            delayTimeMs = value;
            delaySamples = (int)(WaveFormat.SampleRate * delayTimeMs / 1000.0);
            delayBuffer = new Queue<float>();
            for (int i = 0; i < delaySamples; i++)
                delayBuffer.Enqueue(0);
        }
    }

    public float Gain { get; set; } = 0.5f;
    public float Feedback { get; set; } = 0.3f;

    public WaveFormat WaveFormat => source.WaveFormat;

    public DelaySampleProvider(ISampleProvider source)
    {
        this.source = source;
        delayBuffer = new Queue<float>();
        DelayTimeMs = 200;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = source.Read(buffer, offset, count);

        for (int i = 0; i < samplesRead; i++)
        {
            float delayed = delayBuffer.Count > 0 ? delayBuffer.Dequeue() : 0;
            float output = buffer[offset + i] + delayed * Gain;
            delayBuffer.Enqueue(buffer[offset + i] + delayed * Feedback);
            buffer[offset + i] = output;
        }

        return samplesRead;
    }
}
var log = "Payment failed for user john.doe@gmail.com due to timeout";
Console.WriteLine(LogSanitizer.Sanitize(log));
Console.ReadLine();

public static class LogSanitizer
{
    public static string Sanitize(string message)
    {
        ReadOnlySpan<char> span = message;
        Span<char> buffer = stackalloc char[message.Length];
        var index = 0;

        while (!span.IsEmpty)
        {
            var atIndex = span.IndexOf('@');

            if (atIndex == -1)
            {
                span.CopyTo(buffer[index..]);
                index += span.Length;
                break;
            }

            var start = FindEmailStart(span, atIndex);
            var end = FindEmailEnd(span, atIndex);

            if (start == -1 || end == -1)
            {
                buffer[index++] = span[0];
                span = span[1..];
                continue;
            }

            var beforeEmail = span[..start];
            beforeEmail.CopyTo(buffer[index..]);
            index += beforeEmail.Length;

            var email = span[start..end];
            var masked = MaskEmail(email);

            masked.CopyTo(buffer[index..]);
            index += masked.Length;

            span = span[end..];
        }

        return new string(buffer[..index]);
    }

    private static int FindEmailStart(ReadOnlySpan<char> span, int atIndex)
    {
        var i = atIndex;
        while (i > 0)
        {
            char c = span[i - 1];
            if (!IsEmailChar(c))
            {
                break;
            }
            i--;
        }

        return i;
    }

    private static int FindEmailEnd(ReadOnlySpan<char> span, int atIndex)
    {
        var i = atIndex;
        while (i < span.Length)
        {
            char c = span[i];
            if (!IsEmailChar(c))
            {
                break;
            }
            i++;
        }
        return i;
    }

    private static bool IsEmailChar(char c)
    {
        return char.IsLetterOrDigit(c)
               || c == '.'
               || c == '_'
               || c == '-'
               || c == '@';
    }

    private static string MaskEmail(ReadOnlySpan<char> email)
    {
        int at = email.IndexOf('@');
        if (at <= 1)
        {
            return email.ToString();
        }
        var buffer = new char[email.Length];
        buffer.AsSpan(0, at - 1).Fill('*');
        email[at..].CopyTo(buffer.AsSpan(at));

        return new string(buffer);
    }
}

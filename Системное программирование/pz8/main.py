"""
Программа для отображения нажатых клавиш в консоли
"""

import keyboard


def main():
    print("=" * 60)
    print("Программа отслеживания нажатий клавиш")
    print("Нажмите любую клавишу для отображения её названия")
    print("Нажмите ESC для выхода из программы")
    print("=" * 60)
    print()

    running = True

    def on_key_press(event):
        """Обработчик нажатия клавиши"""
        # Используем переменную running из внешней функции
        nonlocal running
        if not running:
            return

        # Получаем название клавиши
        key_name = event.name

        # Специальная обработка для некоторых клавиш
        special_keys = {
            'space': 'ПРОБЕЛ',
            'enter': 'ENTER',
            'tab': 'TAB',
            'backspace': 'BACKSPACE',
            'ctrl': 'CTRL',
            'alt': 'ALT',
            'shift': 'SHIFT',
            'caps lock': 'CAPS LOCK',
            'num lock': 'NUM LOCK',
            'scroll lock': 'SCROLL LOCK',
            'print screen': 'PRINT SCREEN',
            'pause': 'PAUSE',
            'insert': 'INSERT',
            'delete': 'DELETE',
            'home': 'HOME',
            'end': 'END',
            'page up': 'PAGE UP',
            'page down': 'PAGE DOWN',
            'up': 'СТРЕЛКА ВВЕРХ',
            'down': 'СТРЕЛКА ВНИЗ',
            'left': 'СТРЕЛКА ВЛЕВО',
            'right': 'СТРЕЛКА ВПРАВО',
        }

        # Заменяем название на русское, если есть
        display_name = special_keys.get(key_name, key_name.upper())

        # Выводим информацию о нажатой клавише
        print(f"Нажата клавиша: {display_name}")

        # Проверяем выход из программы
        if key_name == 'esc':
            # Используем переменную из внешней функции
            running = False
            print("\nВыход из программы...")

    # Устанавливаем обработчик нажатий
    keyboard.on_press(on_key_press)

    # Ждём нажатия ESC
    keyboard.wait('esc')

    print("Программа завершена.")


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n\nПрограмма прервана пользователем.")
    except Exception as e:
        print(f"\nОшибка: {e}")
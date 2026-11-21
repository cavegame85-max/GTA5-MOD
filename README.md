# OMNIA SA — GTA San Andreas PC 1.01

Первый прототип внутриигрового меню для классической GTA San Andreas на ПК.

## Цель первого этапа
- CLEO-скрипт запускается вместе с игрой.
- Клавиша **F10** переключает отображение тестовой панели OMNIA.
- Пока каталог модов пустой, меню показывает сообщение, что моды не найдены.
- **Esc** пока не закрывает меню: закрытие повторным нажатием F10.

## Совместимость
Целевая версия — GTA San Andreas PC **1.01**. Для запуска нужен установленный CLEO 4, совместимый с exe 1.01, и ASI Loader, который используется выбранной сборкой CLEO. Сам CLEO runtime в наш ZIP не включается.

## Сборка
GitHub Actions скачивает Sanny Builder 4.2.0, компилирует CLEO-скрипт и публикует ZIP-артефакт.

Открой Actions → Build OMNIA SA → последний успешный запуск → Artifacts → `OMNIA-SA-1.01`.

## Ограничения прототипа
Это первый проверочный интерфейс, а не полноценный графический мод-менеджер. Он ещё не сканирует установленные моды и не включает/отключает их. Проверка непосредственно в игре ещё не выполнена.



## OMNIA MouseFix (separate module)

A separate x86 ASI project is being developed on the `omnia-mousefix` branch. See [MouseFix documentation](docs/MOUSEFIX.md). Its first CI milestone is a diagnostic bootstrap; it does not yet claim to repair mouse input. The dedicated workflow is **Build OMNIA MouseFix**.

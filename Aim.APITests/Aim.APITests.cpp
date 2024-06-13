#include <iostream>
#include <Windows.h>
static HWND m_hWnd = 0;
void process_msg(UINT msg, WPARAM wp, LPARAM lp)
{
    char buf[100];
    static int i = 1;
    if (!m_hWnd) {
        return;
    }
    switch (msg) {
    case 0x2cbd:
        std::cout << msg

    }
}

int main()
{
    HWND    hAimWindow;
    hAimWindow = FindWindow(NULL, L"Aim");

    SendMessage(hAimWindow, 0x2cbc, 20, NULL);

    std::cout << hAimWindow;
}

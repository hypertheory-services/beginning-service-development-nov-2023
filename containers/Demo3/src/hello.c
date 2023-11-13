#include <stdio.h>
#include <time.h>

int main(void) {
    time_t t;
    time(&t);

    printf("I'm running in a container Running some freaking C code OMG! It is now %s\n", ctime(&t));
    return (0);
}
cold

compiletoflash


\ enable led ports
: init-motor ( -- )
    $E PORTF_DEN !
    $E PORTF_DIR !
    $E PORTF_DATA ! ;

create pattern $8 c, $C c, $4 c, $6 c, $2 c, $A c,
6 constant pattern#
0 variable pos

50 variable delay

: step ( -- )
    \ increment counter, keep bounds
    pos @ 1+ dup pattern# >= if
        drop 0
    then
    dup pattern + c@ PORTF_DATA !
    pos ! ;

: steps ( n -- )
    0 do
        step
        delay @ us
    loop ;

: init
    init
    init-delay
    init-motor
    ." press any key to abort"
    2000 ms
    key-flush
    begin
        step
        delay @ us
    key? until key drop ;


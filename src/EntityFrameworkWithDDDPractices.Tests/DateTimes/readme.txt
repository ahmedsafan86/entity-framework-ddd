Issues regarding using dateime

- DateTime is working fine in most scenarios, but:
    - need conversion to utc on save and retreive (can be done using efcore conversion)
    - if you need to retreive the stored timezone this is not possible

-> so use DateTimeOffset instead to store zone info
    - DateTimeOffset is not supported in some providers like sqlite
    - se this is a test for how to support it
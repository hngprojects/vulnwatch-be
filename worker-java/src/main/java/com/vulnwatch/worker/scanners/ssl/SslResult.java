package com.vulnwatch.worker.scanners.ssl;

import lombok.Data;
import lombok.NoArgsConstructor;

import java.time.LocalDate;
import java.util.List;

@Data
@NoArgsConstructor
public class SslResult {
    private String issuer;
    private String subject;
    private LocalDate validFrom;
    private LocalDate expiryDate;
    private int daysUntilExpiry;
    private List weakProtocols;
    private List findings;
}

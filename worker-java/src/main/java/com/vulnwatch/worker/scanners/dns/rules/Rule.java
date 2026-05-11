package com.vulnwatch.worker.scanners.dns.rules;

import com.vulnwatch.worker.scanners.dns.models.Finding;
import com.vulnwatch.worker.scanners.dns.models.ScanContext;

import java.util.List;
import java.util.Optional;

public interface Rule {
    List<Finding> evaluate(ScanContext context);
}
